using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


public class PlanetPointerInput : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private PlanetActivity planetActivity;
    [SerializeField] private PlanetHoverView hoverView;
    [SerializeField] private LayerMask layerMask = ~0;
    [SerializeField] private float maxDistance = 1000f;

    private bool isHovered;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        Debug.Log($"PlanetPointerInput awake on {name}. Collider: {GetComponent<Collider>() != null}", this);
    }

    private void Update()
    {
        bool hoveringThisFrame = IsMouseOverThisPlanet();

        if (hoveringThisFrame && !isHovered)
        {
            isHovered = true;
            Debug.Log($"Planet hover enter: {name}", this);
            hoverView?.ShowHover();
        }
        else if (!hoveringThisFrame && isHovered)
        {
            isHovered = false;
            Debug.Log($"Planet hover exit: {name}", this);
            hoverView?.HideHover();
        }

        if (isHovered && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryInteract();
        }
    }

    private bool IsMouseOverThisPlanet()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return false;
        }


        if (targetCamera == null || Mouse.current == null)
        {
            return false;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = targetCamera.ScreenPointToRay(mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask, QueryTriggerInteraction.Collide))
        {
            return false;
        }

        return hit.collider.gameObject == gameObject;
    }

    private void TryInteract()
    {
        Debug.Log($"Planet clicked: {name}", this);

        if (planetActivity == null)
        {
            Debug.LogWarning($"No PlanetActivity assigned on {name}.", this);
            return;
        }

        if (!planetActivity.CanInteract())
        {
            Debug.Log($"Planet cannot interact: {name}", this);
            return;
        }

        planetActivity.Interact();
    }
}
