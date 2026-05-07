using System;
using System.Collections.Generic;
using UnityEngine;

public enum MiningSourceType
{
    Manual,
    Bot,
    Offline,
    Weather,
    Debug
}

public class MiningOperationResult
{
    public bool WasCritical;
    public MiningSourceType SourceType;
    public Vector2Int CenterCoordinate;

    public List<TileMiningResult> TileResults = new List<TileMiningResult>();
    public List<ResourceReward> TotalResourceRewards = new List<ResourceReward>();
    public List<CollectedContentResult> CollectedContents = new List<CollectedContentResult>();

    public bool HasAnyChanges => TileResults.Count > 0;
}

public class TileMiningResult
{
    public Vector2Int Coordinate;
    public double DamageApplied;
    public bool EnemyDamaged;
    public bool EnemyDefeated;
    public bool LayerDamaged;
    public bool LayerBroken;
    public bool TileDepleted;
    public int StartingDepthLevel;
    public int EndingDepthLevel;

    public List<ResourceReward> ResourceRewards = new List<ResourceReward>();
}

[Serializable]
public struct CollectedContentResult
{
    public TileContentType Type;
    public string ContentId;
    public LootRarity Rarity;
    public Vector2Int Coordinate;
    public int DepthLevel;

    public CollectedContentResult(
        TileContentType type,
        string contentId,
        LootRarity rarity,
        Vector2Int coordinate,
        int depthLevel)
    {
        Type = type;
        ContentId = contentId;
        Rarity = rarity;
        Coordinate = coordinate;
        DepthLevel = depthLevel;
    }
}
