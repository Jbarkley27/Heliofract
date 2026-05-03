using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InfoPopupView : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform popupTransform;
    [SerializeField] private Canvas targetCanvas;

    [Header("Follow Mouse")]
    [SerializeField] private bool followMouse = true;
    [SerializeField] private Vector2 mouseOffset = new Vector2(18f, -18f);

    [Header("Content")]
    [SerializeField] private GameObject iconRoot;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private GameObject descriptionRoot;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private GameObject extraRoot;
    [SerializeField] private TMP_Text extraText;

    [Header("Animation")]
    [SerializeField] private Vector3 hiddenScale = new Vector3(0.94f, 0.94f, 1f);
    [SerializeField] private Vector3 visibleScale = Vector3.one;
    [SerializeField] private float showDuration = 0.16f;
    [SerializeField] private float hideDuration = 0.1f;
    [SerializeField] private Ease showEase = Ease.OutCubic;
    [SerializeField] private Ease hideEase = Ease.InQuad;

    private Sequence popupSequence;
    private RectTransform canvasTransform;
    private bool isVisible;

    private void Awake()
    {
        if (targetCanvas == null)
        {
            targetCanvas = GetComponentInParent<Canvas>();
        }

        if (targetCanvas != null)
        {
            canvasTransform = targetCanvas.transform as RectTransform;
        }

        HideImmediate();
    }

    private void LateUpdate()
    {
        if (!isVisible || !followMouse)
        {
            return;
        }

        FollowMouse();
    }

    public void Show(InfoPopupData data)
    {
        SetData(data);
        ForceLayoutRefresh();
        FollowMouse();

        popupSequence?.Kill();
        isVisible = true;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (popupTransform != null)
        {
            popupTransform.localScale = hiddenScale;
        }

        popupSequence = DOTween.Sequence();

        if (canvasGroup != null)
        {
            popupSequence.Join(canvasGroup.DOFade(1f, showDuration));
        }

        if (popupTransform != null)
        {
            popupSequence.Join(popupTransform.DOScale(visibleScale, showDuration).SetEase(showEase));
        }
    }

    public void Hide()
    {
        popupSequence?.Kill();
        isVisible = false;

        popupSequence = DOTween.Sequence();

        if (canvasGroup != null)
        {
            popupSequence.Join(canvasGroup.DOFade(0f, hideDuration));
        }

        if (popupTransform != null)
        {
            popupSequence.Join(popupTransform.DOScale(hiddenScale, hideDuration).SetEase(hideEase));
        }
    }

    public void HideImmediate()
    {
        popupSequence?.Kill();
        isVisible = false;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (popupTransform != null)
        {
            popupTransform.localScale = hiddenScale;
        }
    }

    private void SetData(InfoPopupData data)
    {
        bool hasIcon = data.Icon != null;
        bool hasDescription = !string.IsNullOrWhiteSpace(data.Description);
        bool hasExtra = !string.IsNullOrWhiteSpace(data.Extra);

        if (iconRoot != null)
        {
            iconRoot.SetActive(hasIcon);
        }

        if (iconImage != null)
        {
            iconImage.sprite = data.Icon;
        }

        if (titleText != null)
        {
            titleText.text = data.Title;
        }

        if (descriptionRoot != null)
        {
            descriptionRoot.SetActive(hasDescription);
        }

        if (descriptionText != null)
        {
            descriptionText.text = data.Description;
        }

        if (extraRoot != null)
        {
            extraRoot.SetActive(hasExtra);
        }

        if (extraText != null)
        {
            extraText.text = data.Extra;
        }
    }

    private void ForceLayoutRefresh()
    {
        if (titleText != null)
        {
            titleText.ForceMeshUpdate();
        }

        if (descriptionText != null)
        {
            descriptionText.ForceMeshUpdate();
        }

        if (extraText != null)
        {
            extraText.ForceMeshUpdate();
        }

        if (popupTransform != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(popupTransform);
        }
    }

    private void FollowMouse()
    {
        if (popupTransform == null || targetCanvas == null || Mouse.current == null)
        {
            return;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue() + mouseOffset;

        if (targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            popupTransform.position = mousePosition;
            return;
        }

        if (canvasTransform == null)
        {
            return;
        }

        Camera canvasCamera = targetCanvas.worldCamera;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasTransform,
            mousePosition,
            canvasCamera,
            out Vector2 localPoint
        );

        popupTransform.anchoredPosition = localPoint;
    }

    private void OnDestroy()
    {
        popupSequence?.Kill();
    }
}

public struct InfoPopupData
{
    public Sprite Icon;
    public string Title;
    public string Description;
    public string Extra;

    public InfoPopupData(string title, string description = "", string extra = "", Sprite icon = null)
    {
        Title = title;
        Description = description;
        Extra = extra;
        Icon = icon;
    }
}
