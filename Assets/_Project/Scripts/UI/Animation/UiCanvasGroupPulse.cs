using DG.Tweening;
using UnityEngine;

public class UiCanvasGroupPulse : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private CanvasGroup target;

    [Header("Alpha")]
    [SerializeField, Range(0f, 1f)] private float minAlpha = 0.35f;
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 1f;
    [SerializeField, Min(0.01f)] private float fadeDuration = 0.65f;
    [SerializeField] private Ease ease = Ease.InOutSine;

    [Header("Playback")]
    [SerializeField] private bool playOnEnable = true;
    [SerializeField] private bool resetToMaxAlphaOnStop = false;
    [SerializeField] private bool useUnscaledTime = false;

    private Tween pulseTween;

    private void Awake()
    {
        if (target == null)
        {
            target = GetComponent<CanvasGroup>();
        }
    }

    private void OnEnable()
    {
        if (playOnEnable)
        {
            Play();
        }
    }

    private void OnDisable()
    {
        Stop();
    }

    public void Play()
    {
        if (target == null)
        {
            return;
        }

        Stop();

        target.alpha = maxAlpha;
        pulseTween = target
            .DOFade(minAlpha, fadeDuration)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(useUnscaledTime);
    }

    public void Stop()
    {
        pulseTween?.Kill();
        pulseTween = null;

        if (resetToMaxAlphaOnStop && target != null)
        {
            target.alpha = maxAlpha;
        }
    }
}
