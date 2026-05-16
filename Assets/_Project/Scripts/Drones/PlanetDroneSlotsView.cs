using System.Collections.Generic;
using UnityEngine;

public class PlanetDroneSlotsView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameState gameState;
    [SerializeField] private PlanetActivity planetActivity;
    [SerializeField] private ResourceInventory resourceInventory;

    [Header("Visibility")]
    [SerializeField] private GameObject contentRoot;

    [Header("Drone Data")]
    [SerializeField] private DroneDefinition defaultDroneDefinition;

    [Header("Slots")]
    [SerializeField] private List<PlanetDroneSlotUI> slotViews = new List<PlanetDroneSlotUI>();

    private void Awake()
    {
        if (planetActivity == null)
        {
            planetActivity = GetComponentInParent<PlanetActivity>();
        }

        if (gameState == null)
        {
            gameState = FindFirstObjectByType<GameState>();
        }

        if (resourceInventory == null)
        {
            resourceInventory = FindFirstObjectByType<ResourceInventory>();
        }

        if (contentRoot == null)
        {
            contentRoot = gameObject;
        }

        if (slotViews.Count == 0)
        {
            GetComponentsInChildren(true, slotViews);
        }
    }

    private void Start()
    {
        Refresh();
    }

    private void OnEnable()
    {
        if (planetActivity != null)
        {
            planetActivity.MiningStateChanged += HandleMiningStateChanged;
        }

        if (gameState != null)
        {
            gameState.StateChanged += HandleGameStateChanged;
        }

        if (resourceInventory != null)
        {
            resourceInventory.ResourceAmountsChanged += HandleResourceAmountsChanged;
        }

        SubscribeSlotClicks();
    }

    private void OnDisable()
    {
        if (planetActivity != null)
        {
            planetActivity.MiningStateChanged -= HandleMiningStateChanged;
        }

        if (gameState != null)
        {
            gameState.StateChanged -= HandleGameStateChanged;
        }

        if (resourceInventory != null)
        {
            resourceInventory.ResourceAmountsChanged -= HandleResourceAmountsChanged;
        }

        UnsubscribeSlotClicks();
    }

    public void Refresh()
    {
        bool dronesUnlocked = gameState != null && gameState.DronePurchasingUnlocked;
        SetRoot(contentRoot, dronesUnlocked);

        if (!dronesUnlocked)
        {
            return;
        }

        PlanetMiningState miningState = planetActivity != null ? planetActivity.MiningState : null;

        for (int i = 0; i < slotViews.Count; i++)
        {
            PlanetDroneSlotState slotState = miningState != null && i < miningState.DroneSlots.Count
                ? miningState.DroneSlots[i]
                : null;

            slotViews[i].Refresh(slotState, defaultDroneDefinition, CanAffordNextAction(slotState));
        }
    }

    private void HandleMiningStateChanged(PlanetMiningState state)
    {
        Refresh();
    }

    private void HandleResourceAmountsChanged()
    {
        Refresh();
    }

    private void HandleGameStateChanged()
    {
        Refresh();
    }

    private void SubscribeSlotClicks()
    {
        for (int i = 0; i < slotViews.Count; i++)
        {
            if (slotViews[i] != null)
            {
                slotViews[i].Clicked += HandleSlotClicked;
            }
        }
    }

    private void UnsubscribeSlotClicks()
    {
        for (int i = 0; i < slotViews.Count; i++)
        {
            if (slotViews[i] != null)
            {
                slotViews[i].Clicked -= HandleSlotClicked;
            }
        }
    }

    private void HandleSlotClicked(PlanetDroneSlotUI slotView)
    {
        if (slotView == null)
        {
            return;
        }

        PlanetDroneSlotState slotState = slotView.SlotState;

        if (slotState == null || !slotState.IsUnlocked)
        {
            return;
        }

        if (gameState == null || !gameState.DronePurchasingUnlocked)
        {
            return;
        }

        bool changed = slotState.HasDrone
            ? TryUpgradeDrone(slotState)
            : TryPurchaseDrone(slotState);

        if (changed)
        {
            Refresh();
        }
    }

    private bool TryPurchaseDrone(PlanetDroneSlotState slotState)
    {
        if (defaultDroneDefinition == null)
        {
            Debug.LogWarning($"{name} cannot purchase a drone without a default DroneDefinition.", this);
            return false;
        }

        DroneLevelDefinition firstLevel = defaultDroneDefinition.GetFirstLevel();

        if (firstLevel == null)
        {
            Debug.LogWarning($"{defaultDroneDefinition.name} has no drone levels defined.", defaultDroneDefinition);
            return false;
        }

        if (resourceInventory == null || !resourceInventory.TrySpendCosts(firstLevel.Costs))
        {
            return false;
        }

        slotState.InstallDrone(defaultDroneDefinition);
        return true;
    }

    private bool TryUpgradeDrone(PlanetDroneSlotState slotState)
    {
        DroneLevelDefinition nextLevel = slotState.GetNextLevel();

        if (nextLevel == null)
        {
            return false;
        }

        if (resourceInventory == null || !resourceInventory.TrySpendCosts(nextLevel.Costs))
        {
            return false;
        }

        return slotState.Upgrade();
    }

    private bool CanAffordNextAction(PlanetDroneSlotState slotState)
    {
        if (gameState == null || !gameState.DronePurchasingUnlocked)
        {
            return false;
        }

        if (slotState == null || !slotState.IsUnlocked || resourceInventory == null)
        {
            return false;
        }

        DroneLevelDefinition nextLevel;

        if (slotState.HasDrone)
        {
            nextLevel = slotState.GetNextLevel();
        }
        else
        {
            nextLevel = defaultDroneDefinition != null
                ? defaultDroneDefinition.GetFirstLevel()
                : null;
        }

        return nextLevel != null && resourceInventory.HasCosts(nextLevel.Costs);
    }

    private void SetRoot(GameObject root, bool active)
    {
        if (root != null)
        {
            root.SetActive(active);
        }
    }
}
