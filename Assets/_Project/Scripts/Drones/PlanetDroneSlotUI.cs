using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlanetDroneSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event System.Action<PlanetDroneSlotUI> Clicked;

    [Header("Roots")]
    [SerializeField] private GameObject emptyRoot;
    [SerializeField] private GameObject occupiedRoot;
    [SerializeField] private GameObject upgradeAvailableRoot;
    [SerializeField] private Button button;
    [SerializeField] private bool clearSelectionOnClick = true;

    [Header("Occupied UI")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image rarityAccentImage;
    [SerializeField] private Slider miningRateSlider;
    [SerializeField] private TMP_Text levelText;

    [Header("Module Slots")]
    [SerializeField] private List<GameObject> moduleSlotRoots = new List<GameObject>();

    [Header("Popup")]
    [SerializeField] private DroneInfoPopupView droneInfoPopup;

    [Header("Rarity Colors")]
    [SerializeField] private Color commonColor = new Color(0.75f, 0.82f, 0.9f);
    [SerializeField] private Color uncommonColor = new Color(0.25f, 0.95f, 0.55f);
    [SerializeField] private Color rareColor = new Color(0.25f, 0.55f, 1f);
    [SerializeField] private Color epicColor = new Color(0.65f, 0.3f, 1f);
    [SerializeField] private Color legendaryColor = new Color(1f, 0.72f, 0.22f);
    [SerializeField] private Color exoticColor = new Color(1f, 0.22f, 0.55f);

    private PlanetDroneSlotState slotState;
    private DroneDefinition defaultDroneDefinition;
    private bool canAffordAction;
    private bool isHovered;

    public PlanetDroneSlotState SlotState => slotState;

    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (button == null)
        {
            button = GetComponentInChildren<Button>(true);
        }

        if (droneInfoPopup == null)
        {
            droneInfoPopup = FindFirstObjectByType<DroneInfoPopupView>(FindObjectsInactive.Include);
        }
    }

    private void OnEnable()
    {
        if (button != null)
        {
            button.onClick.AddListener(HandleClicked);
        }
    }

    private void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(HandleClicked);
        }

        HidePopup();
    }

    public void Refresh(
        PlanetDroneSlotState newSlotState,
        DroneDefinition newDefaultDroneDefinition,
        bool newCanAffordAction)
    {
        slotState = newSlotState;
        defaultDroneDefinition = newDefaultDroneDefinition;
        canAffordAction = newCanAffordAction;

        bool isDisabled = slotState == null || !slotState.IsUnlocked;
        bool isEmpty = !isDisabled && !slotState.HasDrone;
        bool isOccupied = !isDisabled && slotState.HasDrone;

        gameObject.SetActive(!isDisabled);

        if (isDisabled)
        {
            return;
        }

        SetRoot(emptyRoot, isEmpty);
        SetRoot(occupiedRoot, isOccupied);
        SetRoot(upgradeAvailableRoot, isOccupied && slotState.GetNextLevel() != null && canAffordAction);

        if (button != null)
        {
            button.interactable = CanClickCurrentState();
        }
        else
        {
            Debug.LogWarning($"{name} is missing a Button reference for drone slot interaction.", this);
        }

        if (isOccupied)
        {
            RefreshOccupiedState();
        }
        else
        {
            RefreshEmptyState();
        }

        if (isHovered)
        {
            ShowPopup();
        }
    }

    private bool CanClickCurrentState()
    {
        if (slotState == null || !slotState.IsUnlocked || !canAffordAction)
        {
            return false;
        }

        if (!slotState.HasDrone)
        {
            return defaultDroneDefinition != null && defaultDroneDefinition.GetFirstLevel() != null;
        }

        return slotState.GetNextLevel() != null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        ShowPopup();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HidePopup();
    }

    private void HandleClicked()
    {
        Clicked?.Invoke(this);

        if (clearSelectionOnClick && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void RefreshOccupiedState()
    {
        DroneLevelDefinition currentLevel = slotState.GetCurrentLevel();

        if (currentLevel == null)
        {
            return;
        }

        Sprite icon = currentLevel.Icon != null ? currentLevel.Icon : slotState.DroneDefinition.Icon;

        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.enabled = icon != null;
        }

        if (rarityAccentImage != null)
        {
            rarityAccentImage.color = GetRarityColor(currentLevel.Rarity);
        }

        if (miningRateSlider != null)
        {
            miningRateSlider.minValue = 0f;
            miningRateSlider.maxValue = 1f;
            miningRateSlider.value = Mathf.Clamp01((float)slotState.MiningProgress01);
        }

        if (levelText != null)
        {
            levelText.text = $"{slotState.Level}/{slotState.DroneDefinition.GetMaxLevel()}";
        }

        RefreshModuleSlots(currentLevel.ModuleSlotsUnlocked);
    }

    private void RefreshEmptyState()
    {
        if (miningRateSlider != null)
        {
            miningRateSlider.value = 0f;
        }

        if (levelText != null)
        {
            DroneLevelDefinition firstLevel = defaultDroneDefinition != null
                ? defaultDroneDefinition.GetFirstLevel()
                : null;

            levelText.text = firstLevel != null
                ? $"0/{defaultDroneDefinition.GetMaxLevel()}"
                : "0/-";
        }

        RefreshModuleSlots(0);
    }

    private void RefreshModuleSlots(int unlockedSlotCount)
    {
        for (int i = 0; i < moduleSlotRoots.Count; i++)
        {
            SetRoot(moduleSlotRoots[i], i < unlockedSlotCount);
        }
    }

    private void ShowPopup()
    {
        if (droneInfoPopup == null)
        {
            return;
        }

        droneInfoPopup.Show(BuildPopupData());
    }

    private void HidePopup()
    {
        isHovered = false;
        droneInfoPopup?.Hide();
    }

    private DroneInfoPopupData BuildPopupData()
    {
        if (slotState == null || !slotState.IsUnlocked)
        {
            return new DroneInfoPopupData
            {
                Name = "Drone Slot",
                Subtitle = "Unavailable"
            };
        }

        if (!slotState.HasDrone)
        {
            return BuildEmptyPopupData();
        }

        return BuildOccupiedPopupData();
    }

    private DroneInfoPopupData BuildEmptyPopupData()
    {
        if (defaultDroneDefinition == null)
        {
            return new DroneInfoPopupData
            {
                Name = "Empty Drone Slot",
                Subtitle = "No drone definition assigned."
            };
        }

        DroneLevelDefinition firstLevel = defaultDroneDefinition.GetFirstLevel();

        if (firstLevel == null)
        {
            return new DroneInfoPopupData
            {
                Icon = defaultDroneDefinition.Icon,
                Name = defaultDroneDefinition.DisplayName,
                Subtitle = "No levels defined."
            };
        }

        return new DroneInfoPopupData
        {
            Icon = firstLevel.Icon != null ? firstLevel.Icon : defaultDroneDefinition.Icon,
            HeaderColor = GetRarityColor(firstLevel.Rarity),
            Name = $"{defaultDroneDefinition.DisplayName}",
            Subtitle = $"{firstLevel.Rarity} LV.{firstLevel.Level}",
            MiningRateValue = FormatRate(firstLevel.MiningRatePerSecond),
            DamageValue = NumberFormatter.FormatNumber(firstLevel.DamagePerHit),
            HasNextUpgrade = false,
            ExtraText = firstLevel.UpgradeDescription,
            ActionText = "Upgrade",
            Costs = firstLevel.Costs
        };
    }

    private DroneInfoPopupData BuildOccupiedPopupData()
    {
        DroneLevelDefinition currentLevel = slotState.GetCurrentLevel();
        DroneLevelDefinition nextLevel = slotState.GetNextLevel();

        string displayName = !string.IsNullOrWhiteSpace(currentLevel.DisplayName)
            ? currentLevel.DisplayName
            : slotState.DroneDefinition.DisplayName;

        Sprite icon = currentLevel.Icon != null ? currentLevel.Icon : slotState.DroneDefinition.Icon;

        return new DroneInfoPopupData
        {
            Icon = icon,
            HeaderColor = GetRarityColor(currentLevel.Rarity),
            Name = displayName,
            Subtitle = $"{currentLevel.Rarity} LV{slotState.Level}",
            MiningRateValue = FormatRate(currentLevel.MiningRatePerSecond),
            DamageValue = NumberFormatter.FormatNumber(currentLevel.DamagePerHit),
            HasNextUpgrade = nextLevel != null,
            NextUpgradeTitle = nextLevel != null ? GetNextUpgradeTitle(nextLevel) : "Max level reached",
            ShowRarityUpgrade = nextLevel != null && nextLevel.Rarity != currentLevel.Rarity,
            CurrentRarityText = currentLevel.Rarity.ToString(),
            NextRarityText = nextLevel != null ? nextLevel.Rarity.ToString() : string.Empty,
            MiningRateUpgradeValue = nextLevel != null ? GetRateUpgradeText(currentLevel, nextLevel) : string.Empty,
            DamageUpgradeValue = nextLevel != null ? GetDamageUpgradeText(currentLevel, nextLevel) : string.Empty,
            NewModuleSlotText = nextLevel != null ? GetNewModuleSlotText(currentLevel, nextLevel) : string.Empty,
            ExtraText = nextLevel != null ? nextLevel.UpgradeDescription : string.Empty,
            ActionText = nextLevel != null ? GetActionText(nextLevel) : string.Empty,
            Costs = nextLevel != null ? nextLevel.Costs : null
        };
    }

    private string GetNextUpgradeTitle(DroneLevelDefinition nextLevel)
    {
        string verb = nextLevel.UpgradeType == DroneUpgradeType.Evolution
            ? "Evolve"
            : nextLevel.UpgradeType == DroneUpgradeType.Ascension
                ? "Ascend"
                : "Next Upgrade";

        string nextName = !string.IsNullOrWhiteSpace(nextLevel.DisplayName)
            ? nextLevel.DisplayName
            : $"Level {nextLevel.Level}";

        return $"{verb}: {nextName}";
    }

    private string GetActionText(DroneLevelDefinition nextLevel)
    {
        switch (nextLevel.UpgradeType)
        {
            case DroneUpgradeType.Evolution:
                return "Evolve";

            case DroneUpgradeType.Ascension:
                return "Ascend";

            case DroneUpgradeType.Normal:
            default:
                return "Upgrade";
        }
    }

    private string GetNewModuleSlotText(DroneLevelDefinition currentLevel, DroneLevelDefinition nextLevel)
    {
        int newSlots = nextLevel.ModuleSlotsUnlocked - currentLevel.ModuleSlotsUnlocked;

        if (newSlots <= 0)
        {
            return string.Empty;
        }

        return newSlots == 1 ? "+1 module slot" : $"+{newSlots} module slots";
    }

    private string GetRateUpgradeText(DroneLevelDefinition currentLevel, DroneLevelDefinition nextLevel)
    {
        double rateIncrease = nextLevel.MiningRatePerSecond - currentLevel.MiningRatePerSecond;

        if (rateIncrease == 0)
        {
            return string.Empty;
        }

        return $"{FormatSignedNumber(rateIncrease)}/s";
    }

    private string GetDamageUpgradeText(DroneLevelDefinition currentLevel, DroneLevelDefinition nextLevel)
    {
        double damageIncrease = nextLevel.DamagePerHit - currentLevel.DamagePerHit;

        if (damageIncrease == 0)
        {
            return string.Empty;
        }

        return FormatSignedNumber(damageIncrease);
    }

    private string FormatRate(double value)
    {
        return $"{NumberFormatter.FormatNumber(value)}/s";
    }

    private string FormatSignedNumber(double value)
    {
        string prefix = value > 0 ? "+" : string.Empty;
        return $"{prefix}{NumberFormatter.FormatNumber(value)}";
    }

    private Color GetRarityColor(LootRarity rarity)
    {
        switch (rarity)
        {
            case LootRarity.Uncommon:
                return uncommonColor;

            case LootRarity.Rare:
                return rareColor;

            case LootRarity.Epic:
                return epicColor;

            case LootRarity.Legendary:
                return legendaryColor;

            case LootRarity.Exotic:
                return exoticColor;

            case LootRarity.Common:
            default:
                return commonColor;
        }
    }

    private void SetRoot(GameObject root, bool active)
    {
        if (root != null)
        {
            root.SetActive(active);
        }
    }
}
