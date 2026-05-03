using DG.Tweening;
using UnityEngine;


public class PlanetHoverView : MonoBehaviour
{
    private static readonly int BrightnessProperty = Shader.PropertyToID("_Brightness");

    [Header("State")]
    [SerializeField] private PlanetActivity planetActivity;

    [Header("Material")]
    [SerializeField] private Renderer planetRenderer;
    [SerializeField] private float unlockedBrightness = 1.2f;
    [SerializeField] private float unlockedHoverBrightness = 1.55f;
    [SerializeField] private float lockedBrightness = 0.5f;
    [SerializeField] private float lockedHoverBrightness = 0.65f;

    [Header("UI")]
    [SerializeField] private CanvasGroup uiRootCanvasGroup;
    [SerializeField] private CanvasGroup nameCanvasGroup;
    [SerializeField] private Transform nameTransform;

    [Header("Idle")]
    [SerializeField, Range(0f, 1f)] private float idleRootAlpha = 0.35f;
    [SerializeField, Range(0f, 1f)] private float idleNameAlpha = 0.75f;
    [SerializeField] private Vector3 idleNameScale = Vector3.one;

    [Header("Hover")]
    [SerializeField, Range(0f, 1f)] private float hoverRootAlpha = 1f;
    [SerializeField, Range(0f, 1f)] private float hoverNameAlpha = 1f;
    [SerializeField] private Vector3 hoverNameScale = new Vector3(1.08f, 1.08f, 1f);

    [Header("Timing")]
    [SerializeField] private float hoverInDuration = 0.18f;
    [SerializeField] private float hoverOutDuration = 0.14f;
    [SerializeField] private Ease hoverInEase = Ease.OutCubic;
    [SerializeField] private Ease hoverOutEase = Ease.OutQuad;

    private Material runtimeMaterial;
    private Sequence hoverSequence;
    private bool isHovered;


    [Header("Popup")]
    [SerializeField] private InfoPopupView infoPopup;
    [SerializeField] private string miningActionText = "Mine";
    [SerializeField] private string lockedPopupTitle = "Locked";
    [SerializeField] private string lockedPopupDescription = "Requires survey access";



    [Header("State Visuals")]
    [SerializeField] private GameObject resourceIconsRoot;
    [SerializeField] private GameObject lockedRoot;



    private void Awake()
    {
        CreateRuntimeMaterial();
    }

    private void OnEnable()
    {
        if (planetActivity != null)
        {
            planetActivity.StateChanged += HandleActivityStateChanged;
        }
    }

    private void Start()
    {
        ApplyIdleState();
        RefreshStateVisuals();
    }

    private void OnDisable()
    {
        if (planetActivity != null)
        {
            planetActivity.StateChanged -= HandleActivityStateChanged;
        }
    }

    public void ShowHover()
    {
        isHovered = true;

        PlayHoverTween(hoverRootAlpha, hoverNameAlpha, hoverNameScale, GetHoverBrightness(), hoverInDuration, hoverInEase);
        ShowInfoPopup();
    }

    public void HideHover()
    {
        isHovered = false;

        ApplyIdleStateAnimated();
        infoPopup?.Hide();
    }


    private void ShowInfoPopup()
    {
        if (infoPopup == null)
        {
            return;
        }

        if (CanInteract())
        {
            infoPopup.Show(new InfoPopupData(miningActionText));
        }
        else
        {
            infoPopup.Show(new InfoPopupData(lockedPopupTitle, lockedPopupDescription));
        }
    }



    private void ApplyIdleState()
    {
        if (uiRootCanvasGroup != null)
        {
            uiRootCanvasGroup.alpha = idleRootAlpha;
        }

        if (nameCanvasGroup != null)
        {
            nameCanvasGroup.alpha = idleNameAlpha;
        }

        if (nameTransform != null)
        {
            nameTransform.localScale = idleNameScale;
        }


        SetBrightness(GetIdleBrightness());
    }

    private void ApplyIdleStateAnimated()
    {
        PlayHoverTween(idleRootAlpha, idleNameAlpha, idleNameScale, GetIdleBrightness(), hoverOutDuration, hoverOutEase);
    }

    private void PlayHoverTween(
        float rootAlpha,
        float nameAlpha,
        Vector3 nameScale,
        float brightness,
        float duration,
        Ease ease)
    {
        hoverSequence?.Kill();

        hoverSequence = DOTween.Sequence();

        if (uiRootCanvasGroup != null)
        {
            hoverSequence.Join(uiRootCanvasGroup.DOFade(rootAlpha, duration));
        }

        if (nameCanvasGroup != null)
        {
            hoverSequence.Join(nameCanvasGroup.DOFade(nameAlpha, duration));
        }

        if (nameTransform != null)
        {
            hoverSequence.Join(nameTransform.DOScale(nameScale, duration).SetEase(ease));
        }

        if (runtimeMaterial != null && runtimeMaterial.HasProperty(BrightnessProperty))
        {
            hoverSequence.Join(
                DOTween.To(
                    () => runtimeMaterial.GetFloat(BrightnessProperty),
                    value => runtimeMaterial.SetFloat(BrightnessProperty, value),
                    brightness,
                    duration
                )
            );
        }

        hoverSequence.SetEase(ease);
    }

    private void CreateRuntimeMaterial()
    {
        if (planetRenderer == null)
        {
            return;
        }

        runtimeMaterial = planetRenderer.material;
    }

    private float GetIdleBrightness()
    {
        return CanInteract() ? unlockedBrightness : lockedBrightness;
    }

    private bool CanInteract()
    {
        return planetActivity != null && planetActivity.CanInteract();
    }

    private void SetBrightness(float brightness)
    {
        if (runtimeMaterial == null || !runtimeMaterial.HasProperty(BrightnessProperty))
        {
            return;
        }

        runtimeMaterial.SetFloat(BrightnessProperty, brightness);
    }

    private void OnDestroy()
    {
        hoverSequence?.Kill();
    }

    private void HandleActivityStateChanged(ActivityState state)
    {
        RefreshStateVisuals();

        if (isHovered)
        {
            ShowHover();
        }
    }


    public void RefreshStateVisuals()
    {
        bool canInteract = CanInteract();

        if (resourceIconsRoot != null)
        {
            resourceIconsRoot.SetActive(canInteract);
        }

        if (lockedRoot != null)
        {
            lockedRoot.SetActive(!canInteract);
        }

        float brightness = isHovered
            ? GetHoverBrightness()
            : GetIdleBrightness();

        SetBrightness(brightness);
    }

    private float GetHoverBrightness()
    {
        return CanInteract() ? unlockedHoverBrightness : lockedHoverBrightness;
    }

}
