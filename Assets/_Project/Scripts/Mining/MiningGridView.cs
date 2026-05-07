using System.Collections.Generic;
using UnityEngine;

public class MiningGridView : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private DepthDatabase depthDatabase;

    [Header("Tile View")]
    [SerializeField] private MiningTileView tilePrefab;
    [SerializeField] private RectTransform tileParent;

    [Header("Layout")]
    [SerializeField] private float tileSize = 48f;
    [SerializeField] private float tileSpacing = 2f;

    private readonly List<MiningTileView> tileViews = new List<MiningTileView>();
    private PlanetMiningState currentState;

    [SerializeField] private MiningToolDefinition miningTool;
    [SerializeField] private ResourceInventory resourceInventory;
    


    public void Show(PlanetMiningState state)
    {
        currentState = state;
        Rebuild();
    }

    public void Refresh()
    {
        if (currentState == null)
        {
            Clear();
            return;
        }

        for (int i = 0; i < tileViews.Count; i++)
        {
            tileViews[i].Refresh(tileViews[i].TileState, depthDatabase);
        }
    }

    public void Clear()
    {
        for (int i = tileViews.Count - 1; i >= 0; i--)
        {
            if (tileViews[i] != null)
            {
                Destroy(tileViews[i].gameObject);
            }
        }

        tileViews.Clear();
        currentState = null;
    }

    private void Rebuild()
    {
        ClearExistingViewsOnly();

        if (currentState == null || tilePrefab == null || tileParent == null)
        {
            return;
        }

        float step = tileSize + tileSpacing;

        for (int i = 0; i < currentState.Tiles.Count; i++)
        {
            MiningTileState tileState = currentState.Tiles[i];
            MiningTileView tileView = Instantiate(tilePrefab, tileParent);

            RectTransform tileRect = tileView.transform as RectTransform;

            if (tileRect != null)
            {
                tileRect.sizeDelta = new Vector2(tileSize, tileSize);
                tileRect.anchoredPosition = new Vector2(
                    tileState.Coordinate.x * step,
                    tileState.Coordinate.y * step
                );
            }

            tileView.Refresh(tileState, depthDatabase);
            tileView.SetClickedCallback(HandleTileClicked);
            tileViews.Add(tileView);
        }
    }

    private void ClearExistingViewsOnly()
    {
        for (int i = tileViews.Count - 1; i >= 0; i--)
        {
            if (tileViews[i] != null)
            {
                Destroy(tileViews[i].gameObject);
            }
        }

        tileViews.Clear();
    }




    private void HandleTileClicked(MiningTileView tileView)
    {
        if (currentState == null || miningTool == null || tileView == null || tileView.TileState == null)
        {
            return;
        }

        MiningOperationResult result = MiningSystem.MineArea(
            currentState,
            tileView.TileState.Coordinate,
            miningTool,
            MiningSourceType.Manual
        );

        ApplyResourceRewards(result);
        RefreshAffectedTiles(result);

        Debug.Log(
            $"Mined {tileView.TileState.Coordinate}. " +
            $"Crit: {result.WasCritical}. " +
            $"Rewards: {FormatRewards(result)}",
            this
        );
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

    private void RefreshAffectedTiles(MiningOperationResult result)
    {
        for (int i = 0; i < result.TileResults.Count; i++)
        {
            MiningTileView tileView = GetTileView(result.TileResults[i].Coordinate);

            if (tileView != null)
            {
                tileView.Refresh(tileView.TileState, depthDatabase);
            }
        }
    }

    private MiningTileView GetTileView(Vector2Int coordinate)
    {
        for (int i = 0; i < tileViews.Count; i++)
        {
            if (tileViews[i] != null &&
                tileViews[i].TileState != null &&
                tileViews[i].TileState.Coordinate == coordinate)
            {
                return tileViews[i];
            }
        }

        return null;
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


}
