using System.Collections.Generic;
using PixelNarval.HPBars;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShipSystemNodeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private ShipRestorationManager restorationManager;
    [SerializeField] private ShipSystemDefinition systemDefinition;

    [Header("UI")]
    [SerializeField] private Button button;
    [SerializeField] private CanvasGroup buttonCanvasGroup;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text tierText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text statusText;

    [Header("Progress")]
    [SerializeField] private TransitionData tierTrackData;
    [SerializeField] private TransitionData tierProgressData;
    [SerializeField] private bool animateProgressChanges = true;

    [Header("Popup")]
    [SerializeField] private InfoPopupView infoPopup;
    [SerializeField] private bool showPopupOnHover = true;

    [Header("State Roots")]
    [SerializeField] private GameObject undiscoveredRoot;
    [SerializeField] private GameObject upgradeAvailableRoot;
    [SerializeField] private GameObject maxedRoot;

    [Header("State Alpha")]
    [SerializeField, Range(0f, 1f)] private float undiscoveredAlpha = 0.45f;
    [SerializeField, Range(0f, 1f)] private float discoveredInactiveAlpha = 0.7f;
    [SerializeField, Range(0f, 1f)] private float activeAlpha = 1f;
    [SerializeField, Range(0f, 1f)] private float maxedAlpha = 1f;

    private bool progressInitialized;
    private bool trackInitialized;
    private bool isHovered;
    private int lastTrackMaxTierCount = -1;
    private int lastProgressMaxTierCount = -1;
    private int lastProgressTier = -1;

    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (buttonCanvasGroup == null)
        {
            buttonCanvasGroup = GetComponent<CanvasGroup>();
        }

        if (tierProgressData == null)
        {
            tierProgressData = GetComponentInChildren<TransitionData>(true);
        }

        if (restorationManager == null)
        {
            restorationManager = FindFirstObjectByType<ShipRestorationManager>();
        }

        if (infoPopup == null)
        {
            infoPopup = FindFirstObjectByType<InfoPopupView>(FindObjectsInactive.Include);
        }
    }

    private void OnEnable()
    {
        if (button != null)
        {
            button.onClick.AddListener(HandleClicked);
        }

        if (restorationManager != null)
        {
            restorationManager.ShipSystemsChanged += Refresh;
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(HandleClicked);
        }

        if (restorationManager != null)
        {
            restorationManager.ShipSystemsChanged -= Refresh;
        }

        infoPopup?.Hide();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!showPopupOnHover || infoPopup == null)
        {
            return;
        }

        isHovered = true;
        infoPopup.Show(BuildPopupData());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!showPopupOnHover || infoPopup == null)
        {
            return;
        }

        isHovered = false;
        infoPopup.Hide();
    }

    public void Refresh()
    {
        ShipSystemState state = restorationManager != null
            ? restorationManager.GetState(systemDefinition)
            : null;

        ShipSystemTierDefinition nextTier = restorationManager != null
            ? restorationManager.GetNextTier(systemDefinition)
            : null;

        bool hasDefinition = systemDefinition != null;
        ShipSystemStatus status = restorationManager != null
            ? restorationManager.GetSystemStatus(systemDefinition)
            : ShipSystemStatus.Undiscovered;

        bool isRevealed = status != ShipSystemStatus.Undiscovered;
        bool isMaxed = status == ShipSystemStatus.Maxed;
        bool canRepair = restorationManager != null && restorationManager.CanRepairNextTier(systemDefinition);

        if (nameText != null)
        {
            nameText.text = hasDefinition ? systemDefinition.DisplayName : "Unassigned";
        }

        if (descriptionText != null)
        {
            descriptionText.text = hasDefinition ? systemDefinition.Description : string.Empty;
        }

        if (tierText != null)
        {
            tierText.text = state != null
                ? $"Tier {state.CurrentTier}/{GetMaxTierCount()}"
                : "Tier -";
        }

        if (costText != null)
        {
            costText.text = GetCostText(nextTier);
            costText.gameObject.SetActive(isRevealed && !isMaxed && !string.IsNullOrWhiteSpace(costText.text));
        }

        if (statusText != null)
        {
            statusText.text = GetStatusText(state, nextTier, isMaxed, canRepair);
        }

        if (iconImage != null)
        {
            iconImage.sprite = hasDefinition ? systemDefinition.Icon : null;
            iconImage.enabled = iconImage.sprite != null;
        }

        if (button != null)
        {
            button.interactable = isRevealed && !isMaxed && nextTier != null;
        }

        RefreshButtonAlpha(status);
        RefreshTierTrack();
        RefreshTierProgress(state);

        SetRoot(undiscoveredRoot, status == ShipSystemStatus.Undiscovered);
        SetRoot(upgradeAvailableRoot, canRepair);
        SetRoot(maxedRoot, status == ShipSystemStatus.Maxed);

        if (isHovered && showPopupOnHover && infoPopup != null)
        {
            // Keeps popup details current if resources change while hovered.
            infoPopup.Show(BuildPopupData());
        }
    }

    private void HandleClicked()
    {
        if (restorationManager == null || systemDefinition == null)
        {
            return;
        }

        restorationManager.TryRepairNextTier(systemDefinition);
    }

    private int GetMaxTierCount()
    {
        return systemDefinition != null ? systemDefinition.Tiers.Count : 0;
    }

    private void RefreshButtonAlpha(ShipSystemStatus status)
    {
        if (buttonCanvasGroup == null)
        {
            return;
        }

        switch (status)
        {
            case ShipSystemStatus.Undiscovered:
                buttonCanvasGroup.alpha = undiscoveredAlpha;
                break;

            case ShipSystemStatus.DiscoveredInactive:
                buttonCanvasGroup.alpha = discoveredInactiveAlpha;
                break;

            case ShipSystemStatus.Active:
                buttonCanvasGroup.alpha = activeAlpha;
                break;

            case ShipSystemStatus.Maxed:
                buttonCanvasGroup.alpha = maxedAlpha;
                break;
        }
    }

    private void RefreshTierProgress(ShipSystemState state)
    {
        if (tierProgressData == null || systemDefinition == null)
        {
            return;
        }

        int maxTierCount = GetMaxTierCount();
        int currentTier = state != null ? state.CurrentTier : 0;

        if (lastProgressMaxTierCount != maxTierCount)
        {
            tierProgressData.SetMaxNumberOnly(maxTierCount);
            lastProgressMaxTierCount = maxTierCount;
            progressInitialized = false;
        }

        if (progressInitialized && lastProgressTier == currentTier)
        {
            return;
        }

        if (!progressInitialized || !animateProgressChanges)
        {
            tierProgressData.ForceCurrentValue(maxTierCount <= 0 ? 0f : (float)currentTier / maxTierCount);
            progressInitialized = true;
            lastProgressTier = currentTier;
            return;
        }

        tierProgressData.StartTransitionToInt(currentTier);
        lastProgressTier = currentTier;
    }

    private void RefreshTierTrack()
    {
        if (tierTrackData == null || systemDefinition == null)
        {
            return;
        }

        int maxTierCount = GetMaxTierCount();

        if (trackInitialized && lastTrackMaxTierCount == maxTierCount)
        {
            return;
        }

        tierTrackData.SetMaxNumberOnly(maxTierCount);
        tierTrackData.ForceCurrentValue(1f);

        lastTrackMaxTierCount = maxTierCount;
        trackInitialized = true;
    }

    private string GetCostText(ShipSystemTierDefinition nextTier)
    {
        if (nextTier == null || nextTier.Costs == null || nextTier.Costs.Count == 0)
        {
            return string.Empty;
        }

        List<string> costParts = new List<string>();

        for (int i = 0; i < nextTier.Costs.Count; i++)
        {
            ResourceCost cost = nextTier.Costs[i];
            costParts.Add($"{NumberFormatter.FormatNumber(cost.Amount)} {cost.Type}");
        }

        return string.Join("  ", costParts);
    }

    private InfoPopupData BuildPopupData()
    {
        if (systemDefinition == null)
        {
            return new InfoPopupData("Unknown System");
        }

        ShipSystemState state = restorationManager != null
            ? restorationManager.GetState(systemDefinition)
            : null;

        ShipSystemTierDefinition nextTier = restorationManager != null
            ? restorationManager.GetNextTier(systemDefinition)
            : null;

        ShipSystemStatus status = restorationManager != null
            ? restorationManager.GetSystemStatus(systemDefinition)
            : ShipSystemStatus.Undiscovered;

        string title = systemDefinition.DisplayName;
        string description = GetPopupDescription(status);
        string extra = GetPopupExtra(state, nextTier, status);

        return new InfoPopupData(title, description, extra, systemDefinition.Icon);
    }

    private string GetPopupDescription(ShipSystemStatus status)
    {
        if (status == ShipSystemStatus.Undiscovered)
        {
            return "System undiscovered.";
        }

        return systemDefinition != null ? systemDefinition.Description : string.Empty;
    }

    private string GetPopupExtra(
        ShipSystemState state,
        ShipSystemTierDefinition nextTier,
        ShipSystemStatus status)
    {
        if (status == ShipSystemStatus.Undiscovered)
        {
            return string.Empty;
        }

        List<string> lines = new List<string>();

        if (state != null)
        {
            lines.Add($"Tier {state.CurrentTier}/{GetMaxTierCount()}");
        }

        if (nextTier != null)
        {
            if (!string.IsNullOrWhiteSpace(nextTier.DisplayName))
            {
                lines.Add($"Next: {nextTier.DisplayName}");
            }

            if (!string.IsNullOrWhiteSpace(nextTier.Description))
            {
                lines.Add(nextTier.Description);
            }

            string cost = GetCostText(nextTier);

            if (!string.IsNullOrWhiteSpace(cost))
            {
                lines.Add($"Cost: {cost}");
            }

            string unlocks = GetUnlockEffectsText(nextTier);

            if (!string.IsNullOrWhiteSpace(unlocks))
            {
                lines.Add(unlocks);
            }
        }
        else
        {
            lines.Add("Fully restored.");
        }

        return string.Join("\n", lines);
    }

    private string GetUnlockEffectsText(ShipSystemTierDefinition tier)
    {
        if (tier == null || tier.UnlockEffects == null || tier.UnlockEffects.Count == 0)
        {
            return string.Empty;
        }

        List<string> lines = new List<string>();

        for (int i = 0; i < tier.UnlockEffects.Count; i++)
        {
            ShipUnlockEffectDefinition effect = tier.UnlockEffects[i];

            if (!string.IsNullOrWhiteSpace(effect.Description))
            {
                lines.Add(effect.Description);
                continue;
            }

            lines.Add($"{effect.Type}: {effect.TargetId}");
        }

        return string.Join("\n", lines);
    }

    private string GetStatusText(
        ShipSystemState state,
        ShipSystemTierDefinition nextTier,
        bool isMaxed,
        bool canRepair)
    {
        if (state == null || !state.IsRevealed)
        {
            return "Locked";
        }

        if (isMaxed)
        {
            return "Fully Restored";
        }

        if (nextTier == null)
        {
            return "No Upgrade Available";
        }

        if (canRepair)
        {
            return state.IsActivated ? "Upgrade Available" : "Repair Available";
        }

        return state.IsActivated ? "Needs Resources" : "Repair Required";
    }

    private void SetRoot(GameObject root, bool active)
    {
        if (root != null)
        {
            root.SetActive(active);
        }
    }
}
