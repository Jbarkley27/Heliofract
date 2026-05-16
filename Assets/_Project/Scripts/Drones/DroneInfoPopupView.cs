using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DroneInfoPopupView : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform popupTransform;
    [SerializeField] private Canvas targetCanvas;

    [Header("Follow Mouse")]
    [SerializeField] private bool followMouse = true;
    [SerializeField] private Vector2 mouseOffset = new Vector2(18f, -18f);

    [Header("Header")]
    [SerializeField] private GameObject headerRoot;
    [SerializeField] private Image headerBackgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text subtitleText;

    [Header("Current Stats")]
    [SerializeField] private GameObject miningRateRoot;
    [SerializeField] private TMP_Text miningRateValueText;
    [SerializeField] private GameObject damageRoot;
    [SerializeField] private TMP_Text damageValueText;

    [Header("Next Upgrade")]
    [SerializeField] private GameObject nextUpgradeRoot;
    [SerializeField] private TMP_Text nextUpgradeTitleText;
    [SerializeField] private GameObject rarityUpgradeRoot;
    [SerializeField] private TMP_Text rarityFromText;
    [SerializeField] private TMP_Text rarityToText;
    [SerializeField] private GameObject miningRateUpgradeRoot;
    [SerializeField] private TMP_Text miningRateUpgradeText;
    [SerializeField] private GameObject damageUpgradeRoot;
    [SerializeField] private TMP_Text damageUpgradeText;
    [SerializeField] private GameObject newSlotRoot;
    [SerializeField] private TMP_Text newSlotText;
    [SerializeField] private GameObject extraRoot;
    [SerializeField] private TMP_Text extraText;

    [Header("Action")]
    [SerializeField] private GameObject upgradeButtonRoot;
    [SerializeField] private TMP_Text upgradeButtonText;
    [SerializeField] private ResourceDatabase resourceDatabase;
    [SerializeField] private List<DroneInfoCostRow> costRows = new List<DroneInfoCostRow>();

    [Header("Animation")]
    [SerializeField] private Vector3 hiddenScale = new Vector3(0.94f, 0.94f, 1f);
    [SerializeField] private Vector3 visibleScale = Vector3.one;
    [SerializeField] private float showDuration = 0.16f;
    [SerializeField] private float hideDuration = 0.1f;
    [SerializeField] private Ease showEase = Ease.OutCubic;
    [SerializeField] private Ease hideEase = Ease.InQuad;

    [Header("Child Reveal")]
    [SerializeField] private bool animateChildren = true;
    [SerializeField] private float childFadeDuration = 0.08f;
    [SerializeField] private float childStagger = 0.025f;
    [SerializeField] private List<CanvasGroup> childRevealGroups = new List<CanvasGroup>();

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

        CacheChildRevealGroups();
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

    public void Show(DroneInfoPopupData data)
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

        PrepareChildRevealGroups();

        popupSequence = DOTween.Sequence();

        if (canvasGroup != null)
        {
            popupSequence.Join(canvasGroup.DOFade(1f, showDuration));
        }

        if (popupTransform != null)
        {
            popupSequence.Join(popupTransform.DOScale(visibleScale, showDuration).SetEase(showEase));
        }

        if (animateChildren)
        {
            AppendChildRevealTweens(popupSequence);
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

        PrepareChildRevealGroups();
    }

    private void SetData(DroneInfoPopupData data)
    {
        SetRoot(headerRoot, true);
        SetImage(iconImage, data.Icon);
        SetText(nameText, data.Name);
        SetText(subtitleText, data.Subtitle);

        if (headerBackgroundImage != null)
        {
            headerBackgroundImage.color = data.HeaderColor;
        }

        SetRoot(miningRateRoot, !string.IsNullOrWhiteSpace(data.MiningRateValue));
        SetText(miningRateValueText, data.MiningRateValue);

        SetRoot(damageRoot, !string.IsNullOrWhiteSpace(data.DamageValue));
        SetText(damageValueText, data.DamageValue);

        SetRoot(nextUpgradeRoot, data.HasNextUpgrade);
        SetText(nextUpgradeTitleText, data.NextUpgradeTitle);

        SetRoot(rarityUpgradeRoot, data.ShowRarityUpgrade);
        SetText(rarityFromText, data.CurrentRarityText);
        SetText(rarityToText, data.NextRarityText);

        SetRoot(miningRateUpgradeRoot, !string.IsNullOrWhiteSpace(data.MiningRateUpgradeValue));
        SetText(miningRateUpgradeText, data.MiningRateUpgradeValue);

        SetRoot(damageUpgradeRoot, !string.IsNullOrWhiteSpace(data.DamageUpgradeValue));
        SetText(damageUpgradeText, data.DamageUpgradeValue);

        SetRoot(newSlotRoot, !string.IsNullOrWhiteSpace(data.NewModuleSlotText));
        SetText(newSlotText, data.NewModuleSlotText);

        SetRoot(extraRoot, !string.IsNullOrWhiteSpace(data.ExtraText));
        SetText(extraText, data.ExtraText);

        SetRoot(upgradeButtonRoot, !string.IsNullOrWhiteSpace(data.ActionText));
        SetText(upgradeButtonText, data.ActionText);

        RefreshCostRows(data.Costs);
    }

    private void RefreshCostRows(IReadOnlyList<ResourceCost> costs)
    {
        for (int i = 0; i < costRows.Count; i++)
        {
            bool hasCost = costs != null && i < costs.Count;
            DroneInfoCostRow row = costRows[i];

            SetRoot(row.Root, hasCost);

            if (!hasCost)
            {
                continue;
            }

            ResourceCost cost = costs[i];
            ResourceDefinition definition = resourceDatabase != null
                ? resourceDatabase.GetDefinition(cost.Type)
                : null;

            SetImage(row.IconImage, definition != null ? definition.Icon : null);

            string resourceName = definition != null && !string.IsNullOrWhiteSpace(definition.DisplayName)
                ? definition.DisplayName
                : cost.Type.ToString();

            SetText(row.AmountText, $"{NumberFormatter.FormatNumber(cost.Amount)} {resourceName}");
        }
    }

    private void CacheChildRevealGroups()
    {
        if (!animateChildren || childRevealGroups.Count > 0 || popupTransform == null)
        {
            return;
        }

        for (int i = 0; i < popupTransform.childCount; i++)
        {
            Transform child = popupTransform.GetChild(i);
            CanvasGroup childGroup = child.GetComponent<CanvasGroup>();

            if (childGroup == null)
            {
                childGroup = child.gameObject.AddComponent<CanvasGroup>();
            }

            childRevealGroups.Add(childGroup);
        }
    }

    private void PrepareChildRevealGroups()
    {
        if (!animateChildren)
        {
            return;
        }

        for (int i = 0; i < childRevealGroups.Count; i++)
        {
            if (childRevealGroups[i] != null)
            {
                childRevealGroups[i].alpha = 0f;
            }
        }
    }

    private void AppendChildRevealTweens(Sequence sequence)
    {
        for (int i = 0; i < childRevealGroups.Count; i++)
        {
            CanvasGroup childGroup = childRevealGroups[i];

            if (childGroup == null || !childGroup.gameObject.activeInHierarchy)
            {
                continue;
            }

            sequence.Insert(
                i * childStagger,
                childGroup.DOFade(1f, childFadeDuration).SetEase(Ease.OutQuad)
            );
        }
    }

    private void ForceLayoutRefresh()
    {
        ForceMeshUpdate(nameText);
        ForceMeshUpdate(subtitleText);
        ForceMeshUpdate(miningRateValueText);
        ForceMeshUpdate(damageValueText);
        ForceMeshUpdate(nextUpgradeTitleText);
        ForceMeshUpdate(rarityFromText);
        ForceMeshUpdate(rarityToText);
        ForceMeshUpdate(miningRateUpgradeText);
        ForceMeshUpdate(damageUpgradeText);
        ForceMeshUpdate(newSlotText);
        ForceMeshUpdate(extraText);
        ForceMeshUpdate(upgradeButtonText);

        for (int i = 0; i < costRows.Count; i++)
        {
            ForceMeshUpdate(costRows[i].AmountText);
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

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasTransform,
            mousePosition,
            targetCanvas.worldCamera,
            out Vector2 localPoint
        );

        popupTransform.anchoredPosition = localPoint;
    }

    private void SetText(TMP_Text text, string value)
    {
        if (text != null)
        {
            text.text = value;
        }
    }

    private void SetImage(Image image, Sprite sprite)
    {
        if (image == null)
        {
            return;
        }

        image.sprite = sprite;
        image.enabled = sprite != null;
    }

    private void SetRoot(GameObject root, bool active)
    {
        if (root != null)
        {
            root.SetActive(active);
        }
    }

    private void ForceMeshUpdate(TMP_Text text)
    {
        if (text != null)
        {
            text.ForceMeshUpdate();
        }
    }

    private void OnDestroy()
    {
        popupSequence?.Kill();
    }
}

[Serializable]
public class DroneInfoCostRow
{
    public GameObject Root;
    public Image IconImage;
    public TMP_Text AmountText;
}

public struct DroneInfoPopupData
{
    public Sprite Icon;
    public Color HeaderColor;
    public string Name;
    public string Subtitle;
    public string MiningRateValue;
    public string DamageValue;
    public bool HasNextUpgrade;
    public string NextUpgradeTitle;
    public bool ShowRarityUpgrade;
    public string CurrentRarityText;
    public string NextRarityText;
    public string MiningRateUpgradeValue;
    public string DamageUpgradeValue;
    public string NewModuleSlotText;
    public string ExtraText;
    public string ActionText;
    public List<ResourceCost> Costs;
}
