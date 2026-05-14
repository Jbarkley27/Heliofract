using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlanetMiningState
{
    public string PlanetId;
    public int SurfaceAccessLevel;
    public int GridWidth;
    public int GridHeight;
    public int MinDepthLevel;
    public int MaxDepthLevel;
    public List<MiningTileState> Tiles = new List<MiningTileState>();
    public MiningStats Stats = new MiningStats();

    public bool IsSurfaceComplete()
    {
        if (Tiles.Count == 0)
        {
            return false;
        }

        for (int i = 0; i < Tiles.Count; i++)
        {
            if (!Tiles[i].IsDepleted)
            {
                return false;
            }
        }

        return true;
    }

    public MiningTileState GetTile(Vector2Int coordinate)
    {
        for (int i = 0; i < Tiles.Count; i++)
        {
            if (Tiles[i].Coordinate == coordinate)
            {
                return Tiles[i];
            }
        }

        return null;
    }
}

[Serializable]
public class MiningTileState
{
    public Vector2Int Coordinate;
    public int CurrentDepthLevel;
    public double CurrentHealth;
    public bool IsDepleted;
    public List<TileLayerState> Layers = new List<TileLayerState>();

    public TileLayerState GetCurrentLayer()
    {
        if (IsDepleted)
        {
            return null;
        }

        for (int i = 0; i < Layers.Count; i++)
        {
            if (Layers[i].DepthLevel == CurrentDepthLevel)
            {
                return Layers[i];
            }
        }

        return null;
    }
}

[Serializable]
public class TileLayerState
{
    public int DepthLevel;
    public double MaxHealth;
    public List<ResourceReward> HitRewards = new List<ResourceReward>();
    public List<ResourceReward> BreakRewards = new List<ResourceReward>();
    public TileLayerContentState Content = TileLayerContentState.None;

    public TileLayerState(int depthLevel, double maxHealth)
    {
        DepthLevel = depthLevel;
        MaxHealth = maxHealth;
    }
}

[Serializable]
public class TileLayerContentState
{
    public TileContentType Type;
    public string ContentId;
    public LootRarity Rarity;
    public bool IsKnown;
    public bool IsResolved;
    public double CurrentHealth;
    public double MaxHealth;
    public string ForcedContentId;

    public bool IsBlocking => Type == TileContentType.Enemy && !IsResolved;

    public static TileLayerContentState None => new TileLayerContentState
    {
        Type = TileContentType.None,
        ContentId = string.Empty,
        Rarity = LootRarity.Common,
        IsKnown = false,
        IsResolved = true,
        CurrentHealth = 0,
        MaxHealth = 0,
        ForcedContentId = string.Empty
    };

    public static TileLayerContentState FromDefinition(TileLayerContentDefinition definition, string forcedContentId = "")
    {
        return new TileLayerContentState
        {
            Type = definition.Type,
            ContentId = definition.ContentId,
            Rarity = definition.Rarity,
            IsKnown = definition.StartsKnown,
            IsResolved = definition.Type == TileContentType.None,
            CurrentHealth = definition.MaxHealth,
            MaxHealth = definition.MaxHealth,
            ForcedContentId = forcedContentId
        };
    }
}

[Serializable]
public class MiningStats
{
    public int MiningActions;
    public int ManualMiningActions;
    public int BotMiningActions;

    public int CriticalHits;

    public int LayersBroken;
    public int LayersBrokenManually;
    public int LayersBrokenByBots;

    public int TilesDepleted;
    public int TilesDepletedManually;
    public int TilesDepletedByBots;

    public int LootCollected;
    public int EnemiesDefeated;
    public int SurfaceAreasCompleted;
    public int SurfaceAccessExpansions;

    public List<LootRarityCount> LootCollectedByRarity = new List<LootRarityCount>();
    public List<EnemyTypeCount> EnemiesDefeatedByType = new List<EnemyTypeCount>();

    public void AddLoot(LootRarity rarity)
    {
        LootCollected++;

        for (int i = 0; i < LootCollectedByRarity.Count; i++)
        {
            if (LootCollectedByRarity[i].Rarity == rarity)
            {
                LootCollectedByRarity[i] = new LootRarityCount(rarity, LootCollectedByRarity[i].Count + 1);
                return;
            }
        }

        LootCollectedByRarity.Add(new LootRarityCount(rarity, 1));
    }

    public void AddEnemyDefeated(string enemyTypeId)
    {
        EnemiesDefeated++;

        for (int i = 0; i < EnemiesDefeatedByType.Count; i++)
        {
            if (EnemiesDefeatedByType[i].EnemyTypeId == enemyTypeId)
            {
                EnemiesDefeatedByType[i] = new EnemyTypeCount(enemyTypeId, EnemiesDefeatedByType[i].Count + 1);
                return;
            }
        }

        EnemiesDefeatedByType.Add(new EnemyTypeCount(enemyTypeId, 1));
    }
}

[Serializable]
public struct LootRarityCount
{
    public LootRarity Rarity;
    public int Count;

    public LootRarityCount(LootRarity rarity, int count)
    {
        Rarity = rarity;
        Count = count;
    }
}

[Serializable]
public struct EnemyTypeCount
{
    public string EnemyTypeId;
    public int Count;

    public EnemyTypeCount(string enemyTypeId, int count)
    {
        EnemyTypeId = enemyTypeId;
        Count = count;
    }
}
