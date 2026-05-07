using UnityEngine;
using UnityEngine.UI;

public class MiningTileView : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Image fillImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private Image contentIconImage;
    [SerializeField] private GameObject depletedRoot;

    [Header("Content Icons")]
    [SerializeField] private Sprite unknownContentSprite;
    [SerializeField] private Sprite enemyContentSprite;
    [SerializeField] private Sprite specialContentSprite;

    [Header("Input")]
    [SerializeField] private Button button;

    private MiningTileState tileState;

    public MiningTileState TileState => tileState;

    private System.Action<MiningTileView> clicked;

    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
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
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        RefreshDepthVisual(depthDatabase);
        RefreshContentIcon();
        RefreshDepletedState();
        RefreshInputState();
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
