using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlanetMiningConfig
{
    [Header("Depth Range")]
    [Min(1)] public int MinDepthLevel = 1;
    [Min(1)] public int MaxDepthLevel = 3;

    [Header("Surface Access")]
    [Min(1)] public int StartingSurfaceAccessLevel = 1;
    [Min(1)] public int MaxSurfaceAccessLevel = 3;

    public List<SurfaceAccessDefinition> SurfaceAccessLevels = new List<SurfaceAccessDefinition>();

    [Header("Planet Modifiers")]
    [Min(0)] public double HealthMultiplier = 1;
    [Min(0)] public double RewardMultiplier = 1;
    [Range(0f, 10f)] public float LootChanceMultiplier = 1;
    [Range(0f, 10f)] public float EnemyChanceMultiplier = 1;

    [Header("Authored Content")]
    public List<ForcedTileContentDefinition> ForcedContents = new List<ForcedTileContentDefinition>();
}

[Serializable]
public struct SurfaceAccessDefinition
{
    [Min(1)] public int AccessLevel;
    [Min(1)] public int Radius;

    public SurfaceAccessDefinition(int accessLevel, int radius)
    {
        AccessLevel = accessLevel;
        Radius = radius;
    }
}

[Serializable]
public struct ForcedTileContentDefinition
{
    public string Id;

    [Header("Placement")]
    [Min(1)] public int SurfaceAccessLevel;
    [Min(1)] public int DepthLevel;
    public bool UseSpecificCoordinate;
    public Vector2Int Coordinate;

    [Header("Content")]
    public TileLayerContentDefinition Content;
    public bool SpawnOnce;
}
