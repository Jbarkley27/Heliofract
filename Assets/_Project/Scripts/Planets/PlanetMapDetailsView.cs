using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlanetMapDetailsView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Activity")]
    [SerializeField] private PlanetActivity planetActivity;

    [Header("Level 1 - Always Visible")]
    [SerializeField] private CanvasGroup alwaysVisibleGroup;
    [SerializeField] private TMP_Text planetNameText;
    [SerializeField] private TMP_Text surfaceIntegrityText;
    [SerializeField] private TMP_Text accessLevelText;
    [SerializeField] private TMP_Text dronesText;

    [Header("Level 2 - Pointer Near Planet")]
    [SerializeField] private CanvasGroup nearbyDetailsGroup;
    [SerializeField] private GameObject planetUpgradeRoot;
    [SerializeField] private GameObject weatherRoot;
    [SerializeField] private TMP_Text lootProgressText;
    [SerializeField] private GameObject depthLegendRoot;
    [SerializeField] private Transform hoverDecorationSpinner;
    [SerializeField] private float spinnerRotationDegreesPerSecond = 18f;

    [Header("Level 3 - Hover Detail Targets")]
    [SerializeField] private CanvasGroup extraDetailsGroup;

    [Header("Locked State")]
    [SerializeField] private GameObject unlockedRoot;
    [SerializeField] private GameObject lockedRoot;
    [SerializeField] private TMP_Text lockedText;
    [SerializeField] private string lockedMessage = "Undiscovered";

    [Header("Timing")]
    [SerializeField] private float detailsFadeDuration = 0.15f;
    [SerializeField] private Ease detailsEase = Ease.OutQuad;

    private Tween nearbyTween;
    private Tween extraTween;
    private bool pointerNear;

    private void Awake()
    {
        if (planetActivity == null)
        {
            planetActivity = GetComponentInParent<PlanetActivity>();
        }

        SetCanvasGroup(nearbyDetailsGroup, 0f, false);
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

        nearbyTween?.Kill();
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
        pointerNear = true;
        ShowNearbyDetails(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerNear = false;
        ShowNearbyDetails(false);
        ShowExtraDetails(false);
    }

    public void Refresh()
    {
        PlanetDefinition definition = planetActivity != null ? planetActivity.Definition : null;
        PlanetMiningState miningState = planetActivity != null ? planetActivity.MiningState : null;
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

        if (lockedText != null)
        {
            lockedText.text = lockedMessage;
        }

        if (unlockedRoot != null)
        {
            unlockedRoot.SetActive(unlocked);
        }

        if (lockedRoot != null)
        {
            lockedRoot.SetActive(!unlocked);
        }

        SetCanvasGroup(alwaysVisibleGroup, 1f, true);

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

        nearbyTween?.Kill();

        if (nearbyDetailsGroup == null)
        {
            return;
        }

        nearbyDetailsGroup.interactable = canShow;
        nearbyDetailsGroup.blocksRaycasts = canShow;
        nearbyTween = nearbyDetailsGroup
            .DOFade(canShow ? 1f : 0f, detailsFadeDuration)
            .SetEase(detailsEase);
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
            return "Loot -- / --";
        }

        int totalContent = 0;
        int resolvedContent = 0;

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

                totalContent++;

                if (content.IsResolved)
                {
                    resolvedContent++;
                }
            }
        }

        return $"Loot {resolvedContent}/{totalContent}";
    }

    private void HandleActivityStateChanged(ActivityState state)
    {
        Refresh();
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
}
