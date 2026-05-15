using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlanetMapDetailsView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Activity")]
    [SerializeField] private PlanetActivity planetActivity;

    [Header("Level 1 - Always Visible")]
    [SerializeField] private List<CanvasGroup> alwaysVisibleGroups = new List<CanvasGroup>();
    [SerializeField] private TMP_Text planetNameText;
    [SerializeField] private TMP_Text surfaceIntegrityText;
    [SerializeField] private TMP_Text accessLevelText;
    [SerializeField] private TMP_Text dronesText;

    [Header("Level 2 - Pointer Near Planet")]
    [SerializeField] private List<CanvasGroup> nearbyDetailsGroups = new List<CanvasGroup>();
    [SerializeField] private GameObject planetUpgradeRoot;
    [SerializeField] private GameObject weatherRoot;
    [SerializeField] private TMP_Text lootProgressText;
    [SerializeField] private GameObject depthLegendRoot;
    [SerializeField] private Transform hoverDecorationSpinner;
    [SerializeField] private float spinnerRotationDegreesPerSecond = 18f;

    [Header("Loot Popup")]
    [SerializeField] private InfoPopupTrigger lootPopupTrigger;

    [Header("Level 3 - Hover Detail Targets")]
    [SerializeField] private CanvasGroup extraDetailsGroup;

    [Header("Activity State Roots")]
    [SerializeField] private GameObject hiddenRoot;
    [SerializeField] private GameObject lockedRoot;
    [SerializeField] private GameObject unlockedRoot;
    [SerializeField] private TMP_Text stateText;
    [SerializeField] private string hiddenMessage = "Undiscovered";
    [SerializeField] private string lockedMessage = "Locked";

    [Header("Timing")]
    [SerializeField] private float detailsFadeDuration = 0.15f;
    [SerializeField] private Ease detailsEase = Ease.OutQuad;

    private readonly List<Tween> nearbyTweens = new List<Tween>();
    private Tween extraTween;
    private bool pointerNear;

    private void Awake()
    {
        if (planetActivity == null)
        {
            planetActivity = GetComponentInParent<PlanetActivity>();
        }

        SetCanvasGroups(nearbyDetailsGroups, 0f, false);
        SetCanvasGroup(extraDetailsGroup, 0f, false);
    }

    private void OnEnable()
    {
        if (planetActivity != null)
        {
            planetActivity.StateChanged += HandleActivityStateChanged;
        }
    }

    private void Start()
    {
        Refresh();
    }

    private void OnDisable()
    {
        if (planetActivity != null)
        {
            planetActivity.StateChanged -= HandleActivityStateChanged;
        }

        KillTweens(nearbyTweens);
        extraTween?.Kill();
    }

    private void Update()
    {
        if (pointerNear && hoverDecorationSpinner != null)
        {
            hoverDecorationSpinner.Rotate(
                0f,
                0f,
                -spinnerRotationDegreesPerSecond * Time.deltaTime,
                Space.Self
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetPointerNear(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetPointerNear(false);
        ShowExtraDetails(false);
    }

    public void SetPointerNear(bool isNear)
    {
        if (pointerNear == isNear)
        {
            return;
        }

        pointerNear = isNear;
        ShowNearbyDetails(isNear);

        if (!isNear)
        {
            ShowExtraDetails(false);
        }
    }

    public void Refresh()
    {
        PlanetDefinition definition = planetActivity != null ? planetActivity.Definition : null;
        PlanetMiningState miningState = planetActivity != null ? planetActivity.MiningState : null;
        ActivityState activityState = planetActivity != null ? planetActivity.State : ActivityState.Hidden;
        bool unlocked = planetActivity != null && planetActivity.CanInteract();

        if (planetNameText != null)
        {
            planetNameText.text = definition != null ? definition.DisplayName : name;
        }

        if (accessLevelText != null)
        {
            accessLevelText.text = miningState != null
                ? $"Access {miningState.SurfaceAccessLevel}"
                : "Access -";
        }

        if (surfaceIntegrityText != null)
        {
            surfaceIntegrityText.text = miningState != null
                ? GetSurfaceIntegrityText(miningState)
                : "-- / --";
        }

        if (dronesText != null)
        {
            dronesText.text = "Drones 0/3";
        }

        if (lootProgressText != null)
        {
            lootProgressText.text = GetLootProgressText(miningState);
        }

        if (lootPopupTrigger != null)
        {
            lootPopupTrigger.SetData(GetLootPopupData(miningState));
        }

        if (stateText != null)
        {
            stateText.text = GetStateMessage(activityState);
        }

        RefreshStateRoots(activityState);

        SetCanvasGroups(alwaysVisibleGroups, unlocked ? 1f : 0f, unlocked);

        if (!unlocked)
        {
            ShowNearbyDetails(false);
            ShowExtraDetails(false);
        }
    }

    public void ShowExtraDetails(bool show)
    {
        extraTween?.Kill();

        if (extraDetailsGroup == null)
        {
            return;
        }

        extraDetailsGroup.interactable = show;
        extraDetailsGroup.blocksRaycasts = show;
        extraTween = extraDetailsGroup
            .DOFade(show ? 1f : 0f, detailsFadeDuration)
            .SetEase(detailsEase);
    }

    private void ShowNearbyDetails(bool show)
    {
        bool canShow = show && planetActivity != null && planetActivity.CanInteract();

        KillTweens(nearbyTweens);

        FadeCanvasGroups(nearbyDetailsGroups, canShow ? 1f : 0f, canShow, nearbyTweens);
    }

    private string GetSurfaceIntegrityText(PlanetMiningState miningState)
    {
        int totalTiles = miningState.Tiles.Count;
        int depletedTiles = 0;

        for (int i = 0; i < miningState.Tiles.Count; i++)
        {
            if (miningState.Tiles[i].IsDepleted)
            {
                depletedTiles++;
            }
        }

        int remainingTiles = totalTiles - depletedTiles;
        return $"{remainingTiles}/{totalTiles}";
    }

    private string GetLootProgressText(PlanetMiningState miningState)
    {
        if (miningState == null)
        {
            return "--/--";
        }

        GetContentCounts(miningState, out PlanetContentCounts counts);
        return $"{counts.LootLeft}/{counts.TotalLoot}";
    }

    private InfoPopupData GetLootPopupData(PlanetMiningState miningState)
    {
        if (miningState == null)
        {
            return new InfoPopupData("Loot", "Loot left --\nTotal loot --");
        }

        GetContentCounts(miningState, out PlanetContentCounts counts);
        return new InfoPopupData("Loot", $"Loot left {counts.LootLeft}\nTotal loot {counts.TotalLoot}");
    }

    private void GetContentCounts(PlanetMiningState miningState, out PlanetContentCounts counts)
    {
        counts = new PlanetContentCounts();

        for (int i = 0; i < miningState.Tiles.Count; i++)
        {
            MiningTileState tile = miningState.Tiles[i];

            for (int layerIndex = 0; layerIndex < tile.Layers.Count; layerIndex++)
            {
                TileLayerContentState content = tile.Layers[layerIndex].Content;

                if (content == null || content.Type == TileContentType.None)
                {
                    continue;
                }

                switch (content.Type)
                {
                    case TileContentType.Loot:
                        counts.TotalLoot++;

                        if (content.IsResolved)
                        {
                            counts.ResolvedLoot++;
                        }
                        break;

                    case TileContentType.Enemy:
                        counts.TotalEnemies++;

                        if (content.IsResolved)
                        {
                            counts.ResolvedEnemies++;
                        }
                        break;
                }
            }
        }
    }

    private void HandleActivityStateChanged(ActivityState state)
    {
        Refresh();
    }

    private void RefreshStateRoots(ActivityState activityState)
    {
        SetRoot(hiddenRoot, activityState == ActivityState.Hidden);
        SetRoot(lockedRoot, activityState == ActivityState.Locked);
        SetRoot(unlockedRoot, activityState == ActivityState.Unlocked || activityState == ActivityState.Active);
    }

    private string GetStateMessage(ActivityState activityState)
    {
        switch (activityState)
        {
            case ActivityState.Hidden:
                return hiddenMessage;

            case ActivityState.Locked:
                return lockedMessage;

            case ActivityState.Unlocked:
            case ActivityState.Active:
                return string.Empty;

            default:
                return string.Empty;
        }
    }

    private void SetCanvasGroup(CanvasGroup canvasGroup, float alpha, bool interactable)
    {
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha = alpha;
        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = interactable;
    }

    private void SetCanvasGroups(List<CanvasGroup> canvasGroups, float alpha, bool interactable)
    {
        for (int i = 0; i < canvasGroups.Count; i++)
        {
            SetCanvasGroup(canvasGroups[i], alpha, interactable);
        }
    }

    private void FadeCanvasGroups(
        List<CanvasGroup> canvasGroups,
        float alpha,
        bool interactable,
        List<Tween> tweens)
    {
        for (int i = 0; i < canvasGroups.Count; i++)
        {
            CanvasGroup canvasGroup = canvasGroups[i];

            if (canvasGroup == null)
            {
                continue;
            }

            canvasGroup.interactable = interactable;
            canvasGroup.blocksRaycasts = interactable;
            tweens.Add(canvasGroup
                .DOFade(alpha, detailsFadeDuration)
                .SetEase(detailsEase));
        }
    }

    private void KillTweens(List<Tween> tweens)
    {
        for (int i = 0; i < tweens.Count; i++)
        {
            tweens[i]?.Kill();
        }

        tweens.Clear();
    }

    private void SetRoot(GameObject root, bool active)
    {
        if (root != null)
        {
            root.SetActive(active);
        }
    }

    private struct PlanetContentCounts
    {
        public int TotalLoot;
        public int ResolvedLoot;
        public int TotalEnemies;
        public int ResolvedEnemies;

        public int LootLeft => TotalLoot - ResolvedLoot;
        public int EnemiesLeft => TotalEnemies - ResolvedEnemies;
    }
}
