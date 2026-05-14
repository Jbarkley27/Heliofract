using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Heliofract/Ship/Ship System Definition")]
public class ShipSystemDefinition : ScriptableObject
{
    public string Id;
    public string DisplayName;
    public Sprite Icon;

    [TextArea]
    public string Description;

    public List<ShipSystemTierDefinition> Tiers = new List<ShipSystemTierDefinition>();
}

[Serializable]
public class ShipSystemTierDefinition
{
    [Min(1)]
    public int Tier = 1;

    public string DisplayName;

    [TextArea]
    public string Description;

    public List<ResourceCost> Costs = new List<ResourceCost>();

    public List<ShipUnlockEffectDefinition> UnlockEffects = new List<ShipUnlockEffectDefinition>();
}

[Serializable]
public class ShipUnlockEffectDefinition
{
    public ShipUnlockEffectType Type;
    public string TargetId;
    
    [TextArea]
    public string Description;

}

public enum ShipUnlockEffectType
{
    UnlockPlanet,
    UnlockDronePurchasing,
    RevealShipSystem,
    UnlockDroneUpgrades,
    UnlockActivity
}
