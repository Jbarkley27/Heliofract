using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Heliofract/Drones/Drone Definition")]
public class DroneDefinition : ScriptableObject
{
    public string Id;
    public string DisplayName;
    public Sprite Icon;

    [TextArea]
    public string Description;

    public List<DroneLevelDefinition> Levels = new List<DroneLevelDefinition>();

    public DroneLevelDefinition GetLevel(int level)
    {
        for (int i = 0; i < Levels.Count; i++)
        {
            if (Levels[i].Level == level)
            {
                return Levels[i];
            }
        }

        return null;
    }

    public DroneLevelDefinition GetFirstLevel()
    {
        return Levels.Count > 0 ? Levels[0] : null;
    }

    public int GetMaxLevel()
    {
        int maxLevel = 0;

        for (int i = 0; i < Levels.Count; i++)
        {
            maxLevel = Mathf.Max(maxLevel, Levels[i].Level);
        }

        return maxLevel;
    }
}

[Serializable]
public class DroneLevelDefinition
{
    [Min(1)] public int Level = 1;
    public string DisplayName;
    public Sprite Icon;
    public LootRarity Rarity = LootRarity.Common;
    public DroneUpgradeType UpgradeType = DroneUpgradeType.Normal;

    [Header("Mining")]
    [Min(0)] public double MiningRatePerSecond = 0.25d;
    [Min(0)] public double DamagePerHit = 1d;

    [Header("Modules")]
    [Range(0, PlanetDroneSlotState.MaxModuleSlots)]
    public int ModuleSlotsUnlocked;

    [Header("Upgrade")]
    public List<ResourceCost> Costs = new List<ResourceCost>();

    [TextArea]
    public string UpgradeDescription;

    [TextArea]
    public string TraitDescription;

    public bool IsAscension => UpgradeType == DroneUpgradeType.Ascension;
    public bool IsEvolution => UpgradeType == DroneUpgradeType.Evolution;
}

public enum DroneUpgradeType
{
    Normal,
    Evolution,
    Ascension
}
