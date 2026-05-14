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
        int accessLevel = config.StartingAccessLevel;

        return CreateStateForAccessLevel(planetDefinition, accessLevel);
    }

    public static PlanetMiningState CreateStateForAccessLevel(
        PlanetDefinition planetDefinition,
        int accessLevel)
    {
        if (planetDefinition == null)
        {
            Debug.LogWarning("Cannot create mining state without a PlanetDefinition.");
            return null;
        }

        PlanetMiningConfig config = planetDefinition.MiningConfig;
        int maxDepthLevel = config.GetMaxDepthForAccessLevel(accessLevel);

        PlanetMiningState state = new PlanetMiningState
        {
            PlanetId = planetDefinition.Id,
            SurfaceAccessLevel = accessLevel,
            GridWidth = config.GridWidth,
            GridHeight = config.GridHeight,
            MinDepthLevel = config.MinDepthLevel,
            MaxDepthLevel = maxDepthLevel
        };

        List<Vector2Int> validCoordinates = GenerateMaskedGridCoordinates(
            config.GridWidth,
            config.GridHeight,
            config.CircleRadiusInCells,
            config.MinimumInsideSamplesForValidTile
        );

        for (int i = 0; i < validCoordinates.Count; i++)
        {
            MiningTileState tile = CreateTileState(
                validCoordinates[i],
                config.MinDepthLevel,
                maxDepthLevel
            );

            state.Tiles.Add(tile);
        }

        ApplyForcedContents(state, config.ForcedContents);

        return state;
    }

    public static List<Vector2Int> GenerateMaskedGridCoordinates(
        int gridWidth,
        int gridHeight,
        float circleRadiusInCells,
        int minimumInsideSamplesForValidTile)
    {
        List<Vector2Int> coordinates = new List<Vector2Int>();

        gridWidth = Mathf.Max(1, gridWidth);
        gridHeight = Mathf.Max(1, gridHeight);
        circleRadiusInCells = Mathf.Max(0.1f, circleRadiusInCells);
        minimumInsideSamplesForValidTile = Mathf.Clamp(minimumInsideSamplesForValidTile, 1, 5);

        Vector2 circleCenter = new Vector2(
            (gridWidth - 1) * 0.5f,
            (gridHeight - 1) * 0.5f
        );

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (CellHasEnoughCircleCoverage(
                    x,
                    y,
                    circleCenter,
                    circleRadiusInCells,
                    minimumInsideSamplesForValidTile))
                {
                    coordinates.Add(new Vector2Int(x, y));
                }
            }
        }

        return coordinates;
    }

    private static bool CellHasEnoughCircleCoverage(
        int x,
        int y,
        Vector2 circleCenter,
        float circleRadius,
        int minimumInsideSamples)
    {
        // Five-point sampling lets partial edge tiles exist visually,
        // while rejecting tiny sliver tiles as mineable targets.
        int insideSamples = 0;

        if (PointInsideCircle(new Vector2(x, y), circleCenter, circleRadius))
        {
            insideSamples++;
        }

        if (PointInsideCircle(new Vector2(x - 0.5f, y - 0.5f), circleCenter, circleRadius))
        {
            insideSamples++;
        }

        if (PointInsideCircle(new Vector2(x + 0.5f, y - 0.5f), circleCenter, circleRadius))
        {
            insideSamples++;
        }

        if (PointInsideCircle(new Vector2(x - 0.5f, y + 0.5f), circleCenter, circleRadius))
        {
            insideSamples++;
        }

        if (PointInsideCircle(new Vector2(x + 0.5f, y + 0.5f), circleCenter, circleRadius))
        {
            insideSamples++;
        }

        return insideSamples >= minimumInsideSamples;
    }

    private static bool PointInsideCircle(
        Vector2 point,
        Vector2 circleCenter,
        float circleRadius)
    {
        return (point - circleCenter).sqrMagnitude <= circleRadius * circleRadius;
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

            if (forcedContent.AccessLevel != state.SurfaceAccessLevel)
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

    private static double GetTemporaryLayerHealth(int depthLevel)
    {
        return 5 + depthLevel * 5;
    }
}
