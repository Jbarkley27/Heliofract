using System;
using System.Collections.Generic;

[Serializable]
public class PlanetDroneSlotState
{
    public const int MaxModuleSlots = 2;

    public int SlotIndex;
    public bool IsUnlocked;
    public DroneDefinition DroneDefinition;
    public int Level;
    public double MiningProgress01;
    public List<string> EquippedModuleIds = new List<string>(MaxModuleSlots);

    public bool HasDrone => DroneDefinition != null && Level > 0;

    public PlanetDroneSlotState(int slotIndex, bool isUnlocked)
    {
        SlotIndex = slotIndex;
        IsUnlocked = isUnlocked;
        Level = 0;

        for (int i = 0; i < MaxModuleSlots; i++)
        {
            EquippedModuleIds.Add(string.Empty);
        }
    }

    public DroneLevelDefinition GetCurrentLevel()
    {
        return HasDrone ? DroneDefinition.GetLevel(Level) : null;
    }

    public DroneLevelDefinition GetNextLevel()
    {
        if (DroneDefinition == null)
        {
            return null;
        }

        int nextLevel = HasDrone ? Level + 1 : 1;
        return DroneDefinition.GetLevel(nextLevel);
    }

    public bool IsMaxLevel()
    {
        return DroneDefinition != null && HasDrone && Level >= DroneDefinition.GetMaxLevel();
    }

    public int GetUnlockedModuleSlotCount()
    {
        DroneLevelDefinition currentLevel = GetCurrentLevel();
        return currentLevel != null ? currentLevel.ModuleSlotsUnlocked : 0;
    }

    public void InstallDrone(DroneDefinition definition)
    {
        DroneDefinition = definition;
        Level = definition != null && definition.GetFirstLevel() != null
            ? definition.GetFirstLevel().Level
            : 0;
        MiningProgress01 = 0;
    }

    public bool Upgrade()
    {
        DroneLevelDefinition nextLevel = GetNextLevel();

        if (nextLevel == null || !HasDrone)
        {
            return false;
        }

        Level = nextLevel.Level;
        return true;
    }
}
