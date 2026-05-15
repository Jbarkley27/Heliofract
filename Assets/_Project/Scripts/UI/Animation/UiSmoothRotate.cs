using DG.Tweening;
using UnityEngine;

public class UiSmoothRotate : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private RectTransform target;

    [Header("Rotation")]
    [SerializeField] private Vector3 rotationAxis = new Vector3(0f, 0f, 1f);
    [SerializeField] private float degreesPerLoop = -360f;
    [SerializeField, Min(0.01f)] private float loopDuration = 12f;
    [SerializeField] private Ease ease = Ease.Linear;

    [Header("Playback")]
    [SerializeField] private bool playOnEnable = true;
    [SerializeField] private bool useUnscaledTime = false;

    private Tween rotationTween;

    private void Awake()
    {
        if (target == null)
        {
            target = transform as RectTransform;
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

        Vector3 rotation = rotationAxis.normalized * degreesPerLoop;

        rotationTween = target
            .DOLocalRotate(rotation, loopDuration, RotateMode.LocalAxisAdd)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Restart)
            .SetUpdate(useUnscaledTime);
    }

    public void Stop()
    {
        rotationTween?.Kill();
        rotationTween = null;
    }
}
