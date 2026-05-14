using UnityEngine;
using UnityEngine.UI;

public class MiningTileView : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Image fillImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private Image contentIconImage;
    [SerializeField] private GameObject depletedRoot;
    [SerializeField] private GameObject nonMineableRoot;

    [Header("Content Icons")]
    [SerializeField] private Sprite unknownContentSprite;
    [SerializeField] private Sprite enemyContentSprite;
    [SerializeField] private Sprite specialContentSprite;

    [Header("Input")]
    [SerializeField] private Button button;
    [SerializeField] private CanvasGroup canvasGroup;

    private MiningTileState tileState;

    public MiningTileState TileState => tileState;

    private System.Action<MiningTileView> clicked;

    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        if (button != null)
        {
            button.onClick.AddListener(HandleClicked);
        }
    }


    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(HandleClicked);
        }
    }

    public void SetClickedCallback(System.Action<MiningTileView> callback)
    {
        clicked = callback;
    }

    private void HandleClicked()
    {
        clicked?.Invoke(this);
    }





    public void Refresh(
        MiningTileState state,
        DepthDatabase depthDatabase)
    {
        tileState = state;

        if (tileState == null)
        {
            RefreshInvalid();
            return;
        }

        RefreshValidRootState();
        RefreshDepthVisual(depthDatabase);
        RefreshContentIcon();
        RefreshDepletedState();
        RefreshInputState();
    }

    public void RefreshInvalid()
    {
        tileState = null;

        // Keep this object active so GridLayoutGroup still reserves the cell.
        // It is visible but non-interactive, which preserves the planet shape
        // without letting invalid edge cells count as mineable tiles.
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (fillImage != null)
        {
            fillImage.enabled = false;
            fillImage.raycastTarget = false;
        }

        if (borderImage != null)
        {
            borderImage.enabled = false;
            borderImage.raycastTarget = false;
        }

        if (contentIconImage != null)
        {
            contentIconImage.enabled = false;
            contentIconImage.raycastTarget = false;
        }

        if (depletedRoot != null)
        {
            depletedRoot.SetActive(false);
        }

        if (nonMineableRoot != null)
        {
            nonMineableRoot.SetActive(true);
        }

        if (button != null)
        {
            button.interactable = false;
        }
    }

    private void RefreshValidRootState()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        if (fillImage != null)
        {
            fillImage.raycastTarget = true;
        }

        if (nonMineableRoot != null)
        {
            nonMineableRoot.SetActive(false);
        }
    }

    private void RefreshDepthVisual(DepthDatabase depthDatabase)
    {
        if (fillImage == null)
        {
            return;
        }

        if (tileState.IsDepleted)
        {
            fillImage.enabled = false;
            return;
        }

        DepthDefinition depth = depthDatabase != null
            ? depthDatabase.GetDepthOrFallback(tileState.CurrentDepthLevel)
            : new DepthDefinition(tileState.CurrentDepthLevel, $"Depth {tileState.CurrentDepthLevel}", Color.white);

        fillImage.enabled = true;
        fillImage.color = depth.Color;
    }

    private void RefreshContentIcon()
    {
        if (contentIconImage == null)
        {
            return;
        }

        contentIconImage.enabled = false;
        contentIconImage.sprite = null;
        contentIconImage.raycastTarget = false;

        if (tileState.IsDepleted)
        {
            return;
        }

        TileLayerState currentLayer = tileState.GetCurrentLayer();

        if (currentLayer == null || currentLayer.Content == null || currentLayer.Content.IsResolved)
        {
            return;
        }

        Sprite icon = GetContentIcon(currentLayer.Content);

        if (icon == null)
        {
            return;
        }

        contentIconImage.sprite = icon;
        contentIconImage.enabled = true;
    }

    private Sprite GetContentIcon(TileLayerContentState content)
    {
        if (!content.IsKnown)
        {
            return unknownContentSprite;
        }

        switch (content.Type)
        {
            case TileContentType.Loot:
                return unknownContentSprite;

            case TileContentType.Enemy:
                return enemyContentSprite;

            case TileContentType.Special:
                return specialContentSprite;

            default:
                return null;
        }
    }

    private void RefreshDepletedState()
    {
        if (depletedRoot != null)
        {
            depletedRoot.SetActive(tileState.IsDepleted);
        }
    }

    private void RefreshInputState()
    {
        if (button != null)
        {
            button.interactable = !tileState.IsDepleted;
        }
    }
}
