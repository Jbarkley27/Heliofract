using DG.Tweening;
using UnityEngine;

public class UiScalePulse : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private RectTransform target;

    [Header("Scale")]
    [SerializeField] private Vector3 baseScale = Vector3.one;
    [SerializeField] private Vector3 pulseScale = new Vector3(1.08f, 1.08f, 1f);
    [SerializeField, Min(0.01f)] private float scaleDuration = 0.55f;
    [SerializeField] private Ease ease = Ease.InOutSine;

    [Header("Playback")]
    [SerializeField] private bool playOnEnable = true;
    [SerializeField] private bool useCurrentScaleAsBaseOnAwake = true;
    [SerializeField] private bool resetToBaseScaleOnStop = true;
    [SerializeField] private bool useUnscaledTime = false;

    private Tween scaleTween;

    private void Awake()
    {
        if (target == null)
        {
            target = transform as RectTransform;
        }

        if (useCurrentScaleAsBaseOnAwake && target != null)
        {
            baseScale = target.localScale;
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

        target.localScale = baseScale;
        scaleTween = target
            .DOScale(pulseScale, scaleDuration)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(useUnscaledTime);
    }

    public void Stop()
    {
        scaleTween?.Kill();
        scaleTween = null;

        if (resetToBaseScaleOnStop && target != null)
        {
            target.localScale = baseScale;
        }
    }
}
