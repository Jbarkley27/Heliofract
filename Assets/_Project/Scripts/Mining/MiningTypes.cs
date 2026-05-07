using System;
using UnityEngine;

[Serializable]
public struct ResourceReward
{
    public ResourceType Type;
    public double Amount;

    public ResourceReward(ResourceType type, double amount)
    {
        Type = type;
        Amount = amount;
    }
}

public enum LootRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Exotic
}

public enum TileContentType
{
    None,
    Loot,
    Enemy,
    Special
}

[Serializable]
public struct TileLayerContentDefinition
{
    public TileContentType Type;
    public string ContentId;
    public LootRarity Rarity;
    public bool StartsKnown;
    public double MaxHealth;

    public bool IsBlocking => Type == TileContentType.Enemy;

    public static TileLayerContentDefinition None => new TileLayerContentDefinition
    {
        Type = TileContentType.None,
        ContentId = string.Empty,
        Rarity = LootRarity.Common,
        StartsKnown = false,
        MaxHealth = 0
    };
}

[Serializable]
public struct MiningToolPatternOffset
{
    public Vector2Int Offset;
    public double DamageMultiplier;

    public MiningToolPatternOffset(Vector2Int offset, double damageMultiplier)
    {
        Offset = offset;
        DamageMultiplier = damageMultiplier;
    }
}
