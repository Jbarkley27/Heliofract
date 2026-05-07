using UnityEngine;
using UnityEngine.InputSystem;

public class MiningDebugTester : MonoBehaviour
{
    [SerializeField] private PlanetDefinition planetDefinition;
    [SerializeField] private MiningToolDefinition miningTool;
    [SerializeField] private Vector2Int testCoordinate = Vector2Int.zero;

    private PlanetMiningState miningState;

    [SerializeField] private ResourceInventory resourceInventory;


    private void Start()
    {
        if (planetDefinition == null || miningTool == null)
        {
            Debug.LogWarning("MiningDebugTester needs a PlanetDefinition and MiningToolDefinition.", this);
            return;
        }

        miningState = MiningStateFactory.CreateInitialState(planetDefinition);

        Debug.Log(
            $"Mining debug state created for {planetDefinition.DisplayName}. " +
            $"Tiles: {miningState.Tiles.Count}. " +
            $"Depths: {miningState.MinDepthLevel}-{miningState.MaxDepthLevel}. " +
            $"Access: {miningState.SurfaceAccessLevel}",
            this
        );

        LogTileState(testCoordinate);
    }

    private void Update()
    {
        if (Keyboard.current == null || miningState == null)
        {
            return;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            MineTestCoordinate();
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            LogTileState(testCoordinate);
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            LogStats();
        }
    }

    private void MineTestCoordinate()
    {
        MiningOperationResult result = MiningSystem.MineArea(
            miningState,
            testCoordinate,
            miningTool,
            MiningSourceType.Manual
        );

        ApplyResourceRewards(result);


        Debug.Log(
            $"Mined {testCoordinate}. " +
            $"Crit: {result.WasCritical}. " +
            $"Affected Tiles: {result.TileResults.Count}. " +
            $"Rewards: {FormatRewards(result)}",
            this
        );

        for (int i = 0; i < result.TileResults.Count; i++)
        {
            TileMiningResult tileResult = result.TileResults[i];

            Debug.Log(
                $"Tile {tileResult.Coordinate}: " +
                $"Damage {tileResult.DamageApplied}, " +
                $"Depth {tileResult.StartingDepthLevel}->{tileResult.EndingDepthLevel}, " +
                $"LayerBroken: {tileResult.LayerBroken}, " +
                $"Depleted: {tileResult.TileDepleted}, " +
                $"EnemyDamaged: {tileResult.EnemyDamaged}, " +
                $"EnemyDefeated: {tileResult.EnemyDefeated}",
                this
            );
        }

        LogTileState(testCoordinate);
    }

    private void LogTileState(Vector2Int coordinate)
    {
        if (miningState == null)
        {
            return;
        }

        MiningTileState tile = miningState.GetTile(coordinate);

        if (tile == null)
        {
            Debug.Log($"Tile {coordinate} does not exist.", this);
            return;
        }

        TileLayerState layer = tile.GetCurrentLayer();

        if (tile.IsDepleted || layer == null)
        {
            Debug.Log($"Tile {coordinate} is depleted.", this);
            return;
        }

        string contentText = $"{layer.Content.Type} {layer.Content.ContentId}";

        if (layer.Content.Type == TileContentType.Enemy && !layer.Content.IsResolved)
        {
            contentText += $" HP {layer.Content.CurrentHealth}/{layer.Content.MaxHealth}";
        }

        Debug.Log(
            $"Tile {coordinate}: " +
            $"Depth {tile.CurrentDepthLevel}, " +
            $"HP {tile.CurrentHealth}/{layer.MaxHealth}, " +
            $"Content {contentText}",
            this
        );


    }

    private void LogStats()
    {
        MiningStats stats = miningState.Stats;

        Debug.Log(
            $"Mining Stats | " +
            $"Actions: {stats.MiningActions}, " +
            $"Manual: {stats.ManualMiningActions}, " +
            $"Crits: {stats.CriticalHits}, " +
            $"Layers Broken: {stats.LayersBroken}, " +
            $"Tiles Depleted: {stats.TilesDepleted}, " +
            $"Loot: {stats.LootCollected}, " +
            $"Enemies: {stats.EnemiesDefeated}, " +
            $"Surface Complete: {miningState.IsSurfaceComplete()}",
            this
        );
    }

    private string FormatRewards(MiningOperationResult result)
    {
        if (result.TotalResourceRewards.Count == 0)
        {
            return "None";
        }

        string text = string.Empty;

        for (int i = 0; i < result.TotalResourceRewards.Count; i++)
        {
            ResourceReward reward = result.TotalResourceRewards[i];

            if (i > 0)
            {
                text += ", ";
            }

            text += $"{reward.Amount} {reward.Type}";
        }

        return text;
    }


    private void ApplyResourceRewards(MiningOperationResult result)
    {
        if (resourceInventory == null)
        {
            return;
        }

        for (int i = 0; i < result.TotalResourceRewards.Count; i++)
        {
            ResourceReward reward = result.TotalResourceRewards[i];
            resourceInventory.AddAmount(reward.Type, reward.Amount);
        }
    }

}
