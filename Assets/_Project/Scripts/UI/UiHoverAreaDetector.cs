using UnityEngine;
using UnityEngine.InputSystem;

public class UiHoverAreaDetector : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private PlanetMapDetailsView detailsView;

    [Header("Area")]
    [SerializeField] private RectTransform centerRoot;
    [SerializeField] private UiHoverAreaShape shape = UiHoverAreaShape.Circle;
    [SerializeField] private Vector2 centerOffset;
    [SerializeField, Min(0f)] private float radius = 180f;
    [SerializeField] private Vector2 size = new Vector2(360f, 260f);

    [Header("Canvas")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Camera eventCamera;

    [Header("Debug")]
    [SerializeField] private bool logStateChanges;

    private bool pointerInside;

    private void Awake()
    {
        if (detailsView == null)
        {
            detailsView = GetComponentInParent<PlanetMapDetailsView>();
        }

        if (centerRoot == null)
        {
            centerRoot = transform as RectTransform;
        }

        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }

        if (eventCamera == null && canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            eventCamera = canvas.worldCamera;
        }
    }

    private void OnDisable()
    {
        SetPointerInside(false);
    }

    private void Update()
    {
        if (Mouse.current == null || centerRoot == null)
        {
            SetPointerInside(false);
            return;
        }

        Vector2 pointerPosition = Mouse.current.position.ReadValue();

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                centerRoot,
                pointerPosition,
                eventCamera,
                out Vector2 localPointerPosition))
        {
            SetPointerInside(false);
            return;
        }

        SetPointerInside(IsInsideArea(localPointerPosition));
    }

    private bool IsInsideArea(Vector2 localPointerPosition)
    {
        Vector2 relativePosition = localPointerPosition - centerOffset;

        switch (shape)
        {
            case UiHoverAreaShape.Rectangle:
                Vector2 halfSize = size * 0.5f;
                return Mathf.Abs(relativePosition.x) <= halfSize.x &&
                       Mathf.Abs(relativePosition.y) <= halfSize.y;

            case UiHoverAreaShape.Circle:
            default:
                return relativePosition.sqrMagnitude <= radius * radius;
        }
    }

    private void SetPointerInside(bool inside)
    {
        if (pointerInside == inside)
        {
            return;
        }

        pointerInside = inside;

        if (detailsView != null)
        {
            detailsView.SetPointerNear(pointerInside);
        }

        if (logStateChanges)
        {
            Debug.Log($"{name} hover area active: {pointerInside}", this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        RectTransform rectTransform = centerRoot != null ? centerRoot : transform as RectTransform;

        if (rectTransform == null)
        {
            return;
        }

        Gizmos.color = pointerInside ? Color.cyan : Color.gray;
        Gizmos.matrix = rectTransform.localToWorldMatrix;

        if (shape == UiHoverAreaShape.Rectangle)
        {
            Gizmos.DrawWireCube(centerOffset, new Vector3(size.x, size.y, 0f));
            return;
        }

        Gizmos.DrawWireSphere(centerOffset, radius);
    }
}

public enum UiHoverAreaShape
{
    Circle,
    Rectangle
}
