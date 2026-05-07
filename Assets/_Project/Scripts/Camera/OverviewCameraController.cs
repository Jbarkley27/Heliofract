using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class OverviewCameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform controlledTransform;

    [Header("Movement")]
    [SerializeField] private float maxPanSpeed = 8f;
    [SerializeField] private float keyboardPanSpeed = 8f;
    [SerializeField] private float smoothTime = 0.12f;

    [Header("Edge Scroll")]
    [SerializeField] private bool edgeScrollEnabled = true;
    [SerializeField] private float edgeSizePixels = 80f;
    [SerializeField] private bool pauseEdgeScrollOverUI = true;

    [Header("Bounds")]
    [SerializeField] private bool clampToBounds = true;
    [SerializeField] private Vector2 minBounds = new Vector2(-20f, -12f);
    [SerializeField] private Vector2 maxBounds = new Vector2(20f, 12f);

    [Header("Focus")]
    [SerializeField] private Vector2 focusOffset;

    private Vector3 desiredPosition;
    private Vector3 smoothVelocity;

    private void Awake()
    {
        if (controlledTransform == null)
        {
            controlledTransform = transform;
        }

        desiredPosition = controlledTransform.position;
        desiredPosition = ClampPosition(desiredPosition);
        controlledTransform.position = desiredPosition;
    }

    private void Update()
    {
        Vector2 input = GetKeyboardInput();

        if (edgeScrollEnabled && !IsPointerOverUI())
        {
            input += GetEdgeScrollInput();
        }

        if (input.sqrMagnitude > 1f)
        {
            input.Normalize();
        }

        if (input.sqrMagnitude > 0f)
        {
            desiredPosition += new Vector3(input.x, input.y, 0f) * maxPanSpeed * Time.deltaTime;
            desiredPosition = ClampPosition(desiredPosition);
        }
    }

    private void LateUpdate()
    {
        if (controlledTransform == null)
        {
            return;
        }

        controlledTransform.position = Vector3.SmoothDamp(
            controlledTransform.position,
            desiredPosition,
            ref smoothVelocity,
            smoothTime
        );
    }

    public void FocusActivity(Activity activity)
    {
        if (activity == null)
        {
            return;
        }

        FocusTransform(activity.transform);
    }

    public void FocusTransform(Transform target)
    {
        if (target == null || controlledTransform == null)
        {
            return;
        }

        desiredPosition = new Vector3(
            target.position.x + focusOffset.x,
            target.position.y + focusOffset.y,
            controlledTransform.position.z
        );

        desiredPosition = ClampPosition(desiredPosition);
    }

    private Vector2 GetKeyboardInput()
    {
        if (Keyboard.current == null)
        {
            return Vector2.zero;
        }

        Vector2 input = Vector2.zero;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            input.x -= 1f;
        }

        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            input.x += 1f;
        }

        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
        {
            input.y -= 1f;
        }

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
        {
            input.y += 1f;
        }

        return input * (keyboardPanSpeed / Mathf.Max(maxPanSpeed, 0.01f));
    }

    private Vector2 GetEdgeScrollInput()
    {
        if (Mouse.current == null || edgeSizePixels <= 0f)
        {
            return Vector2.zero;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 input = Vector2.zero;

        if (mousePosition.x < edgeSizePixels)
        {
            input.x = -GetEdgeStrength(mousePosition.x);
        }
        else if (mousePosition.x > Screen.width - edgeSizePixels)
        {
            input.x = GetEdgeStrength(Screen.width - mousePosition.x);
        }

        if (mousePosition.y < edgeSizePixels)
        {
            input.y = -GetEdgeStrength(mousePosition.y);
        }
        else if (mousePosition.y > Screen.height - edgeSizePixels)
        {
            input.y = GetEdgeStrength(Screen.height - mousePosition.y);
        }

        return input;
    }

    private float GetEdgeStrength(float distanceFromEdge)
    {
        return Mathf.Clamp01(1f - distanceFromEdge / edgeSizePixels);
    }

    private bool IsPointerOverUI()
    {
        return pauseEdgeScrollOverUI
            && EventSystem.current != null
            && EventSystem.current.IsPointerOverGameObject();
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        if (!clampToBounds)
        {
            return position;
        }

        position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        position.y = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);
        return position;
    }
}
