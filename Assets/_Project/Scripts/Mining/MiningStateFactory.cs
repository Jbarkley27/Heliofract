using System.Collections.Generic;
using UnityEngine;

public static class MiningStateFactory
{
    public static PlanetMiningState CreateInitialState(PlanetDefinition planetDefinition)
    {
        if (planetDefinition == null)
        {
            Debug.LogWarning("Cannot create mining state without a PlanetDefinition.");
            return null;
        }

        PlanetMiningConfig config = planetDefinition.MiningConfig;
        int surfaceAccessLevel = config.StartingSurfaceAccessLevel;

        return CreateStateForSurfaceAccess(planetDefinition, surfaceAccessLevel);
    }

    public static PlanetMiningState CreateStateForSurfaceAccess(
        PlanetDefinition planetDefinition,
        int surfaceAccessLevel)
    {
        if (planetDefinition == null)
        {
            Debug.LogWarning("Cannot create mining state without a PlanetDefinition.");
            return null;
        }

        PlanetMiningConfig config = planetDefinition.MiningConfig;
        SurfaceAccessDefinition surfaceAccess = GetSurfaceAccessDefinition(config, surfaceAccessLevel);

        PlanetMiningState state = new PlanetMiningState
        {
            PlanetId = planetDefinition.Id,
            SurfaceAccessLevel = surfaceAccess.AccessLevel,
            MinDepthLevel = config.MinDepthLevel,
            MaxDepthLevel = config.MaxDepthLevel
        };

        List<Vector2Int> coordinates = GenerateCircleCoordinates(surfaceAccess.Radius);

        for (int i = 0; i < coordinates.Count; i++)
        {
            MiningTileState tile = CreateTileState(coordinates[i], config.MinDepthLevel, config.MaxDepthLevel);
            state.Tiles.Add(tile);
        }

        ApplyForcedContents(state, config.ForcedContents);

        return state;
    }



    private static void ApplyForcedContents(
        PlanetMiningState state,
        List<ForcedTileContentDefinition> forcedContents)
    {
        if (forcedContents == null)
        {
            return;
        }

        for (int i = 0; i < forcedContents.Count; i++)
        {
            ForcedTileContentDefinition forcedContent = forcedContents[i];

            if (forcedContent.SurfaceAccessLevel != state.SurfaceAccessLevel)
            {
                continue;
            }

            ApplyForcedContent(state, forcedContent);
        }
    }

    private static void ApplyForcedContent(
        PlanetMiningState state,
        ForcedTileContentDefinition forcedContent)
    {
        if (string.IsNullOrWhiteSpace(forcedContent.Id))
        {
            Debug.LogWarning("Forced content is missing an Id.");
            return;
        }

        if (forcedContent.DepthLevel < state.MinDepthLevel || forcedContent.DepthLevel > state.MaxDepthLevel)
        {
            Debug.LogWarning(
                $"Forced content {forcedContent.Id} has invalid depth {forcedContent.DepthLevel}. " +
                $"Planet depth range is {state.MinDepthLevel}-{state.MaxDepthLevel}."
            );
            return;
        }

        MiningTileState tile = forcedContent.UseSpecificCoordinate
            ? state.GetTile(forcedContent.Coordinate)
            : GetFirstValidTileForForcedContent(state, forcedContent);

        if (tile == null)
        {
            Debug.LogWarning(
                $"Forced content {forcedContent.Id} could not find a valid tile. " +
                $"Coordinate: {forcedContent.Coordinate}, UseSpecificCoordinate: {forcedContent.UseSpecificCoordinate}."
            );
            return;
        }

        TileLayerState layer = GetLayer(tile, forcedContent.DepthLevel);

        if (layer == null)
        {
            Debug.LogWarning(
                $"Forced content {forcedContent.Id} could not find depth {forcedContent.DepthLevel} " +
                $"on tile {tile.Coordinate}."
            );
            return;
        }

        if (layer.Content != null && layer.Content.Type != TileContentType.None && !layer.Content.IsResolved)
        {
            Debug.LogWarning(
                $"Forced content {forcedContent.Id} could not be placed because " +
                $"tile {tile.Coordinate} depth {layer.DepthLevel} already has content {layer.Content.ContentId}."
            );
            return;
        }

        layer.Content = TileLayerContentState.FromDefinition(
            forcedContent.Content,
            forcedContent.Id
        );

        Debug.Log(
            $"Placed forced content {forcedContent.Id}: " +
            $"{forcedContent.Content.Type} {forcedContent.Content.ContentId} " +
            $"at tile {tile.Coordinate}, depth {forcedContent.DepthLevel}."
        );
    }

    private static MiningTileState GetFirstValidTileForForcedContent(
        PlanetMiningState state,
        ForcedTileContentDefinition forcedContent)
    {
        for (int i = 0; i < state.Tiles.Count; i++)
        {
            MiningTileState tile = state.Tiles[i];
            TileLayerState layer = GetLayer(tile, forcedContent.DepthLevel);

            if (layer == null)
            {
                continue;
            }

            if (layer.Content == null || layer.Content.Type == TileContentType.None || layer.Content.IsResolved)
            {
                return tile;
            }
        }

        return null;
    }

    



    

    private static TileLayerState GetLayer(MiningTileState tile, int depthLevel)
    {
        for (int i = 0; i < tile.Layers.Count; i++)
        {
            if (tile.Layers[i].DepthLevel == depthLevel)
            {
                return tile.Layers[i];
            }
        }

        return null;
    }







    public static List<Vector2Int> GenerateCircleCoordinates(int radius)
    {
        List<Vector2Int> coordinates = new List<Vector2Int>();

        radius = Mathf.Max(1, radius);
        int radiusSquared = radius * radius;

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                int distanceSquared = x * x + y * y;

                if (distanceSquared <= radiusSquared)
                {
                    coordinates.Add(new Vector2Int(x, y));
                }
            }
        }

        return coordinates;
    }

    private static MiningTileState CreateTileState(
        Vector2Int coordinate,
        int minDepthLevel,
        int maxDepthLevel)
    {
        MiningTileState tile = new MiningTileState
        {
            Coordinate = coordinate,
            CurrentDepthLevel = minDepthLevel,
            IsDepleted = false
        };

        for (int depth = minDepthLevel; depth <= maxDepthLevel; depth++)
        {
            // Temporary health until MiningBalanceDatabase exists.
            double maxHealth = GetTemporaryLayerHealth(depth);
            TileLayerState layer = new TileLayerState(depth, maxHealth);

            // Temporary rewards until MiningBalanceDatabase exists.
            layer.HitRewards.Add(new ResourceReward(ResourceType.Ore, 1));
            layer.BreakRewards.Add(new ResourceReward(ResourceType.Ore, depth * 5));

            tile.Layers.Add(layer);
        }

        TileLayerState firstLayer = tile.GetCurrentLayer();

        if (firstLayer != null)
        {
            tile.CurrentHealth = firstLayer.MaxHealth;
        }

        return tile;
    }

    private static SurfaceAccessDefinition GetSurfaceAccessDefinition(
        PlanetMiningConfig config,
        int surfaceAccessLevel)
    {
        for (int i = 0; i < config.SurfaceAccessLevels.Count; i++)
        {
            if (config.SurfaceAccessLevels[i].AccessLevel == surfaceAccessLevel)
            {
                return config.SurfaceAccessLevels[i];
            }
        }

        Debug.LogWarning(
            $"Missing surface access level {surfaceAccessLevel}. " +
            "Using fallback radius 3."
        );

        return new SurfaceAccessDefinition(surfaceAccessLevel, 3);
    }

    private static double GetTemporaryLayerHealth(int depthLevel)
    {
        return 5 + depthLevel * 5;
    }
}
