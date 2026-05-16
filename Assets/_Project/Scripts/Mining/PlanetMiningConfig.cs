using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlanetMiningConfig
{
    [Header("Grid Shape")]
    [Min(1)] public int GridWidth = 10;
    [Min(1)] public int GridHeight = 10;

    [Tooltip("Circle radius measured in tile-cell units. Partial edge tiles count as valid if they overlap this circle.")]
    [Min(0.1f)] public float CircleRadiusInCells = 5f;

    [Tooltip("A tile is mineable only if this many sample points are inside the circle. Higher values remove tiny edge sliver tiles.")]
    [Range(1, 5)] public int MinimumInsideSamplesForValidTile = 3;

    [Header("Depth Range")]
    [Min(1)] public int MinDepthLevel = 1;

    [Header("Access")]
    [Min(1)] public int StartingAccessLevel = 1;
    [Min(1)] public int MaxAccessLevel = 3;

    public List<PlanetAccessDefinition> AccessLevels = new List<PlanetAccessDefinition>();

    [Header("Drones")]
    [Range(0, 5)] public int MaxDroneSlots = 3;

    [Header("Planet Modifiers")]
    [Min(0)] public double HealthMultiplier = 1;
    [Min(0)] public double RewardMultiplier = 1;
    [Range(0f, 10f)] public float LootChanceMultiplier = 1;
    [Range(0f, 10f)] public float EnemyChanceMultiplier = 1;

    [Header("Authored Content")]
    public List<ForcedTileContentDefinition> ForcedContents = new List<ForcedTileContentDefinition>();

    public int GetMaxDepthForAccessLevel(int accessLevel)
    {
        for (int i = 0; i < AccessLevels.Count; i++)
        {
            if (AccessLevels[i].AccessLevel == accessLevel)
            {
                return AccessLevels[i].MaxDepthLevel;
            }
        }

        Debug.LogWarning(
            $"Missing access level {accessLevel}. " +
            $"Using MinDepthLevel {MinDepthLevel} as fallback max depth."
        );

        return MinDepthLevel;
    }
}

[Serializable]
public struct PlanetAccessDefinition
{
    [Min(1)] public int AccessLevel;
    [Min(1)] public int MaxDepthLevel;

    public PlanetAccessDefinition(int accessLevel, int maxDepthLevel)
    {
        AccessLevel = accessLevel;
        MaxDepthLevel = maxDepthLevel;
    }
}

[Serializable]
public struct ForcedTileContentDefinition
{
    public string Id;

    [Header("Placement")]
    [Min(1)] public int AccessLevel;
    [Min(1)] public int DepthLevel;
    public bool UseSpecificCoordinate;
    public Vector2Int Coordinate;

    [Header("Content")]
    public TileLayerContentDefinition Content;
    public bool SpawnOnce;
}
