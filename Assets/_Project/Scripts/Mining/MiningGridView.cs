using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiningGridView : MonoBehaviour
{
    public event System.Action<PlanetMiningState> MiningStateChanged;

    [Header("Data")]
    [SerializeField] private DepthDatabase depthDatabase;

    [Header("Tile View")]
    [SerializeField] private MiningTileView tilePrefab;
    [SerializeField] private RectTransform tileParent;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;

    [Header("Layout")]
    [SerializeField] private float tileSize = 48f;
    [SerializeField] private float tileSpacing = 2f;
    [SerializeField] private RectTransform circleBorder;
    [SerializeField] private Vector2 circleBorderPadding = Vector2.zero;

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
            MiningTileView tileView = tileViews[i];

            if (tileView == null)
            {
                continue;
            }

            if (tileView.TileState == null)
            {
                tileView.RefreshInvalid();
                continue;
            }

            tileView.Refresh(tileView.TileState, depthDatabase);
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

        ConfigureGridLayout();
        ResizeGridVisuals();

        // Build the full rectangular grid so circular masks still get stable cell spacing.
        // Cells outside the generated planet surface are invisible placeholders.
        for (int y = currentState.GridHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < currentState.GridWidth; x++)
            {
                Vector2Int coordinate = new Vector2Int(x, y);
                MiningTileState tileState = currentState.GetTile(coordinate);
                MiningTileView tileView = Instantiate(tilePrefab, tileParent);

                RectTransform tileRect = tileView.transform as RectTransform;

                if (tileRect != null)
                {
                    tileRect.sizeDelta = new Vector2(tileSize, tileSize);
                }

                if (tileState == null)
                {
                    tileView.RefreshInvalid();
                }
                else
                {
                    tileView.Refresh(tileState, depthDatabase);
                    tileView.SetClickedCallback(HandleTileClicked);
                }

                tileViews.Add(tileView);
            }
        }
    }

    private void ConfigureGridLayout()
    {
        if (gridLayoutGroup == null && tileParent != null)
        {
            gridLayoutGroup = tileParent.GetComponent<GridLayoutGroup>();
        }

        if (gridLayoutGroup == null)
        {
            return;
        }

        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = currentState.GridWidth;
        gridLayoutGroup.cellSize = new Vector2(tileSize, tileSize);
        gridLayoutGroup.spacing = new Vector2(tileSpacing, tileSpacing);
    }

    private void ResizeGridVisuals()
    {
        Vector2 gridSize = GetGridVisualSize();

        if (tileParent != null)
        {
            tileParent.sizeDelta = gridSize;
        }

        if (circleBorder != null)
        {
            circleBorder.sizeDelta = gridSize + circleBorderPadding;
        }
    }

    private Vector2 GetGridVisualSize()
    {
        float width = currentState.GridWidth * tileSize + Mathf.Max(0, currentState.GridWidth - 1) * tileSpacing;
        float height = currentState.GridHeight * tileSize + Mathf.Max(0, currentState.GridHeight - 1) * tileSpacing;

        return new Vector2(width, height);
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
        MiningStateChanged?.Invoke(currentState);

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
