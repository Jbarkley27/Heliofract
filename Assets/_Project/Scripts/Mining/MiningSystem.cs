using System.Collections.Generic;
using UnityEngine;

public static class MiningSystem
{
    public static bool LogMiningEvents;

    public static MiningOperationResult MineArea(
        PlanetMiningState planetState,
        Vector2Int centerCoordinate,
        MiningToolDefinition tool,
        MiningSourceType sourceType)
    {
        MiningOperationResult result = new MiningOperationResult
        {
            CenterCoordinate = centerCoordinate,
            SourceType = sourceType
        };

        if (planetState == null || tool == null)
        {
            Debug.LogWarning("Cannot mine without a PlanetMiningState and MiningToolDefinition.");
            return result;
        }

        result.WasCritical = RollCritical(tool);

        IReadOnlyList<MiningToolPatternOffset> pattern = tool.GetPattern();

        for (int i = 0; i < pattern.Count; i++)
        {
            MiningToolPatternOffset patternOffset = pattern[i];
            Vector2Int targetCoordinate = centerCoordinate + patternOffset.Offset;

            MiningTileState tile = planetState.GetTile(targetCoordinate);

            if (tile == null || tile.IsDepleted)
            {
                continue;
            }

            double damage = tool.BaseDamage * patternOffset.DamageMultiplier;

            if (result.WasCritical)
            {
                damage *= tool.CritDamageMultiplier;
            }

            TileMiningResult tileResult = MineTile(
                planetState,
                tile,
                damage,
                tool,
                result.WasCritical,
                sourceType
            );

            if (tileResult != null)
            {
                result.TileResults.Add(tileResult);
                AddRewards(result.TotalResourceRewards, tileResult.ResourceRewards);
            }
        }

        UpdateMiningActionStats(planetState, sourceType, result.WasCritical);

        return result;
    }

    private static TileMiningResult MineTile(
        PlanetMiningState planetState,
        MiningTileState tile,
        double damage,
        MiningToolDefinition tool,
        bool wasCritical,
        MiningSourceType sourceType)
    {
        TileLayerState currentLayer = tile.GetCurrentLayer();

        if (currentLayer == null)
        {
            return null;
        }

        TileMiningResult result = new TileMiningResult
        {
            Coordinate = tile.Coordinate,
            DamageApplied = damage,
            StartingDepthLevel = tile.CurrentDepthLevel,
            EndingDepthLevel = tile.CurrentDepthLevel
        };

        if (currentLayer.Content != null && currentLayer.Content.IsBlocking)
        {
            DamageBlockingContent(planetState, tile, currentLayer, damage, result);
            return result;
        }

        result.LayerDamaged = true;

        AddLayerHitRewards(currentLayer, result, tool, wasCritical);
        ApplyDamageToLayer(planetState, tile, currentLayer, damage, tool, sourceType, result);

        result.EndingDepthLevel = tile.CurrentDepthLevel;

        return result;
    }

    private static void DamageBlockingContent(
        PlanetMiningState planetState,
        MiningTileState tile,
        TileLayerState layer,
        double damage,
        TileMiningResult result)
    {
        TileLayerContentState content = layer.Content;

        content.CurrentHealth = System.Math.Max(0, content.CurrentHealth - damage);
        result.EnemyDamaged = true;

        if (content.CurrentHealth > 0)
        {
            return;
        }

        content.IsResolved = true;
        result.EnemyDefeated = true;

        planetState.Stats.AddEnemyDefeated(content.ContentId);

        if (LogMiningEvents)
        {
            Debug.Log($"Enemy defeated: {content.ContentId} on tile {tile.Coordinate} depth {layer.DepthLevel}");
        }
    }

    private static void AddLayerHitRewards(
        TileLayerState layer,
        TileMiningResult result,
        MiningToolDefinition tool,
        bool wasCritical)
    {
        for (int i = 0; i < layer.HitRewards.Count; i++)
        {
            ResourceReward reward = layer.HitRewards[i];

            if (wasCritical)
            {
                reward.Amount *= tool.CritRewardMultiplier;
            }

            result.ResourceRewards.Add(reward);
        }
    }

    private static void ApplyDamageToLayer(
        PlanetMiningState planetState,
        MiningTileState tile,
        TileLayerState layer,
        double damage,
        MiningToolDefinition tool,
        MiningSourceType sourceType,
        TileMiningResult result)
    {
        double remainingDamage = damage;

        while (remainingDamage > 0 && !tile.IsDepleted)
        {
            TileLayerState currentLayer = tile.GetCurrentLayer();

            if (currentLayer == null)
            {
                tile.IsDepleted = true;
                result.TileDepleted = true;
                return;
            }

            double damageToLayer = System.Math.Min(tile.CurrentHealth, remainingDamage);
            tile.CurrentHealth -= damageToLayer;
            remainingDamage -= damageToLayer;

            if (tile.CurrentHealth > 0)
            {
                return;
            }

            BreakLayer(planetState, tile, currentLayer, sourceType, result);

            if (!tool.CarryoverDamageEnabled)
            {
                return;
            }
        }
    }

    private static void BreakLayer(
        PlanetMiningState planetState,
        MiningTileState tile,
        TileLayerState layer,
        MiningSourceType sourceType,
        TileMiningResult result)
    {
        result.LayerBroken = true;
        AddRewards(result.ResourceRewards, layer.BreakRewards);
        UpdateLayerBreakStats(planetState, sourceType);

        ResolveNonBlockingContent(planetState, tile, layer);

        int nextDepth = tile.CurrentDepthLevel + 1;

        if (nextDepth > planetState.MaxDepthLevel)
        {
            tile.IsDepleted = true;
            result.TileDepleted = true;
            UpdateTileDepletedStats(planetState, sourceType);
            return;
        }

        tile.CurrentDepthLevel = nextDepth;

        TileLayerState nextLayer = tile.GetCurrentLayer();

        if (nextLayer == null)
        {
            tile.IsDepleted = true;
            result.TileDepleted = true;
            UpdateTileDepletedStats(planetState, sourceType);
            return;
        }

        tile.CurrentHealth = nextLayer.MaxHealth;
    }

    private static void ResolveNonBlockingContent(
        PlanetMiningState planetState,
        MiningTileState tile,
        TileLayerState layer)
    {
        if (layer.Content == null || layer.Content.IsResolved)
        {
            return;
        }

        if (layer.Content.Type == TileContentType.Loot)
        {
            layer.Content.IsResolved = true;
            planetState.Stats.AddLoot(layer.Content.Rarity);

            if (LogMiningEvents)
            {
                Debug.Log(
                    $"Loot collected: {layer.Content.ContentId} " +
                    $"({layer.Content.Rarity}) on tile {tile.Coordinate} depth {layer.DepthLevel}"
                );
            }
        }
        else if (layer.Content.Type == TileContentType.Special)
        {
            layer.Content.IsResolved = true;

            if (LogMiningEvents)
            {
                Debug.Log(
                    $"Special content resolved: {layer.Content.ContentId} " +
                    $"on tile {tile.Coordinate} depth {layer.DepthLevel}"
                );
            }
        }
    }

    private static bool RollCritical(MiningToolDefinition tool)
    {
        return Random.value < tool.CritChance;
    }

    private static void AddRewards(List<ResourceReward> target, List<ResourceReward> rewards)
    {
        for (int i = 0; i < rewards.Count; i++)
        {
            AddReward(target, rewards[i]);
        }
    }

    private static void AddReward(List<ResourceReward> target, ResourceReward reward)
    {
        for (int i = 0; i < target.Count; i++)
        {
            if (target[i].Type == reward.Type)
            {
                target[i] = new ResourceReward(target[i].Type, target[i].Amount + reward.Amount);
                return;
            }
        }

        target.Add(reward);
    }

    private static void UpdateMiningActionStats(
        PlanetMiningState planetState,
        MiningSourceType sourceType,
        bool wasCritical)
    {
        planetState.Stats.MiningActions++;

        if (sourceType == MiningSourceType.Bot)
        {
            planetState.Stats.BotMiningActions++;
        }
        else if (sourceType == MiningSourceType.Manual)
        {
            planetState.Stats.ManualMiningActions++;
        }

        if (wasCritical)
        {
            planetState.Stats.CriticalHits++;
        }
    }

    private static void UpdateLayerBreakStats(
        PlanetMiningState planetState,
        MiningSourceType sourceType)
    {
        planetState.Stats.LayersBroken++;

        if (sourceType == MiningSourceType.Bot)
        {
            planetState.Stats.LayersBrokenByBots++;
        }
        else if (sourceType == MiningSourceType.Manual)
        {
            planetState.Stats.LayersBrokenManually++;
        }
    }

    private static void UpdateTileDepletedStats(
        PlanetMiningState planetState,
        MiningSourceType sourceType)
    {
        planetState.Stats.TilesDepleted++;

        if (sourceType == MiningSourceType.Bot)
        {
            planetState.Stats.TilesDepletedByBots++;
        }
        else if (sourceType == MiningSourceType.Manual)
        {
            planetState.Stats.TilesDepletedManually++;
        }

        if (planetState.IsSurfaceComplete())
        {
            planetState.Stats.SurfaceAreasCompleted++;
            if (LogMiningEvents)
            {
                Debug.Log($"Surface access level {planetState.SurfaceAccessLevel} complete for {planetState.PlanetId}");
            }
        }
    }
}
