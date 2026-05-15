using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipRestorationManager : MonoBehaviour
{
    public event Action ShipSystemsChanged;
    public event Action<ShipSystemDefinition, ShipSystemTierDefinition> ShipSystemTierRepaired;

    [Header("References")]
    [SerializeField] private GameState gameState;
    [SerializeField] private ResourceInventory resourceInventory;

    [Header("Systems")]
    [SerializeField] private List<ShipSystemDefinition> systemDefinitions = new List<ShipSystemDefinition>();
    [SerializeField] private List<string> initiallyRevealedSystemIds = new List<string>();

    [Header("Runtime State")]
    [SerializeField] private List<ShipSystemState> systemStates = new List<ShipSystemState>();

    public IReadOnlyList<ShipSystemDefinition> SystemDefinitions => systemDefinitions;
    public IReadOnlyList<ShipSystemState> SystemStates => systemStates;

    private void Awake()
    {
        if (gameState == null)
        {
            gameState = FindFirstObjectByType<GameState>();
        }

        InitializeStates(); 
    }


    private void Start()
    {
        ShipSystemsChanged?.Invoke();
    }



    private void OnEnable()
    {
        if (resourceInventory != null)
        {
            resourceInventory.ResourceAmountsChanged += HandleResourceAmountsChanged;
        }
    }

    private void OnDisable()
    {
        if (resourceInventory != null)
        {
            resourceInventory.ResourceAmountsChanged -= HandleResourceAmountsChanged;
        }
    }

    public ShipSystemState GetState(ShipSystemDefinition definition)
    {
        if (definition == null)
        {
            return null;
        }

        return GetState(definition.Id);
    }

    public ShipSystemState GetState(string systemId)
    {
        for (int i = 0; i < systemStates.Count; i++)
        {
            if (systemStates[i].SystemId == systemId)
            {
                return systemStates[i];
            }
        }

        return null;
    }

    public ShipSystemTierDefinition GetNextTier(ShipSystemDefinition definition)
    {
        ShipSystemState state = GetState(definition);

        if (definition == null || state == null)
        {
            return null;
        }

        int nextTierNumber = state.GetNextTierNumber();

        for (int i = 0; i < definition.Tiers.Count; i++)
        {
            if (definition.Tiers[i].Tier == nextTierNumber)
            {
                return definition.Tiers[i];
            }
        }

        return null;
    }

    public bool CanRepairNextTier(ShipSystemDefinition definition)
    {
        ShipSystemState state = GetState(definition);
        ShipSystemTierDefinition nextTier = GetNextTier(definition);

        if (state == null || nextTier == null || !state.IsRevealed)
        {
            return false;
        }

        return HasCosts(nextTier.Costs);
    }

    public ShipSystemStatus GetSystemStatus(ShipSystemDefinition definition)
    {
        ShipSystemState state = GetState(definition);

        if (definition == null || state == null || !state.IsRevealed)
        {
            return ShipSystemStatus.Undiscovered;
        }

        if (state.CurrentTier >= definition.Tiers.Count)
        {
            return ShipSystemStatus.Maxed;
        }

        if (!state.IsActivated)
        {
            return ShipSystemStatus.DiscoveredInactive;
        }

        return ShipSystemStatus.Active;
    }

    public float GetTierProgress01(ShipSystemDefinition definition)
    {
        ShipSystemState state = GetState(definition);

        if (definition == null || state == null || definition.Tiers.Count == 0)
        {
            return 0f;
        }

        return Mathf.Clamp01((float)state.CurrentTier / definition.Tiers.Count);
    }

    public bool TryRepairNextTier(ShipSystemDefinition definition)
    {
        ShipSystemState state = GetState(definition);
        ShipSystemTierDefinition nextTier = GetNextTier(definition);

        if (state == null || nextTier == null)
        {
            return false;
        }

        if (!state.IsRevealed)
        {
            Debug.Log($"Cannot repair hidden ship system {definition.DisplayName}.", this);
            return false;
        }

        if (!HasCosts(nextTier.Costs))
        {
            Debug.Log($"Not enough resources to repair {definition.DisplayName} Tier {nextTier.Tier}.", this);
            return false;
        }

        SpendCosts(nextTier.Costs);

        state.CurrentTier = nextTier.Tier;
        state.IsActivated = state.CurrentTier > 0;

        ApplyUnlockEffects(nextTier.UnlockEffects);

        ShipSystemTierRepaired?.Invoke(definition, nextTier);
        ShipSystemsChanged?.Invoke();

        Debug.Log($"Repaired {definition.DisplayName} Tier {nextTier.Tier}.", this);
        return true;
    }

    public void RevealSystem(string systemId)
    {
        ShipSystemState state = GetState(systemId);

        if (state == null || state.IsRevealed)
        {
            return;
        }

        state.IsRevealed = true;
        ShipSystemsChanged?.Invoke();
    }

    private void InitializeStates()
    {
        for (int i = 0; i < systemDefinitions.Count; i++)
        {
            ShipSystemDefinition definition = systemDefinitions[i];

            if (definition == null || string.IsNullOrWhiteSpace(definition.Id))
            {
                continue;
            }

            if (GetState(definition.Id) != null)
            {
                continue;
            }

            bool isRevealed = initiallyRevealedSystemIds.Contains(definition.Id);
            systemStates.Add(new ShipSystemState(definition.Id, isRevealed));
        }
    }

    private bool HasCosts(List<ResourceCost> costs)
    {
        if (resourceInventory == null)
        {
            return false;
        }

        for (int i = 0; i < costs.Count; i++)
        {
            ResourceCost cost = costs[i];

            if (resourceInventory.GetAmount(cost.Type) < cost.Amount)
            {
                return false;
            }
        }

        return true;
    }

    private void SpendCosts(List<ResourceCost> costs)
    {
        List<ResourceAmount> spendAmounts = new List<ResourceAmount>();

        for (int i = 0; i < costs.Count; i++)
        {
            ResourceCost cost = costs[i];
            spendAmounts.Add(new ResourceAmount(cost.Type, -cost.Amount));
        }

        resourceInventory.AddAmounts(spendAmounts);
    }

    private void ApplyUnlockEffects(List<ShipUnlockEffectDefinition> unlockEffects)
    {
        for (int i = 0; i < unlockEffects.Count; i++)
        {
            ShipUnlockEffectDefinition effect = unlockEffects[i];

            switch (effect.Type)
            {
                case ShipUnlockEffectType.UnlockPlanet:
                    UnlockPlanet(effect.TargetId);
                    break;

                case ShipUnlockEffectType.UnlockActivity:
                    UnlockActivity(effect.TargetId);
                    break;

                case ShipUnlockEffectType.UnlockDronePurchasing:
                    UnlockDronePurchasing();
                    break;

                case ShipUnlockEffectType.UnlockDroneUpgrades:
                    UnlockDroneUpgrades();
                    break;

                case ShipUnlockEffectType.RevealShipSystem:
                    RevealSystem(effect.TargetId);
                    break;

                default:
                    Debug.Log($"Unlock effect pending implementation: {effect.Type} -> {effect.TargetId}", this);
                    break;
            }
        }
    }

    private void UnlockPlanet(string planetId)
    {
        if (gameState == null || string.IsNullOrWhiteSpace(planetId))
        {
            return;
        }

        PlanetActivity[] planets = FindObjectsByType<PlanetActivity>(FindObjectsSortMode.None);

        for (int i = 0; i < planets.Length; i++)
        {
            PlanetActivity planet = planets[i];

            if (planet == null || planet.Definition == null || planet.Definition.Id != planetId)
            {
                continue;
            }

            gameState.UnlockPlanet(planet.Definition);
            planet.State = ActivityState.Unlocked;
            return;
        }

        // Keep the activity id unlocked even if the scene node is added later.
        gameState.AddUnlockedActivity(planetId);
    }

    private void UnlockActivity(string activityId)
    {
        if (gameState == null || string.IsNullOrWhiteSpace(activityId))
        {
            return;
        }

        gameState.AddUnlockedActivity(activityId);
    }

    private void UnlockDronePurchasing()
    {
        gameState?.UnlockDronePurchasing();
    }

    private void UnlockDroneUpgrades()
    {
        gameState?.UnlockDroneUpgrades();
    }

    private void HandleResourceAmountsChanged()
    {
        ShipSystemsChanged?.Invoke();
    }
}
