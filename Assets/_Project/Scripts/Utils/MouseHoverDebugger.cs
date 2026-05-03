using UnityEngine;
using UnityEngine.InputSystem;

public class MouseHoverDebugger : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private LayerMask layerMask = ~0;
    [SerializeField] private float maxDistance = 1000f;

    private GameObject lastHoveredObject;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (targetCamera == null || Mouse.current == null)
        {
            Debug.LogWarning("MouseHoverDebugger: Target camera or mouse input not set up correctly.");
            return;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = targetCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask, QueryTriggerInteraction.Collide))
        {
            GameObject hoveredObject = hit.collider.gameObject;

            if (hoveredObject != lastHoveredObject)
            {
                lastHoveredObject = hoveredObject;

                Debug.Log(
                    $"Mouse over: {hoveredObject.name} | Layer: {LayerMask.LayerToName(hoveredObject.layer)} | Collider: {hit.collider.GetType().Name}",
                    hoveredObject
                );
            }

            Debug.DrawLine(ray.origin, hit.point, Color.green);
        }
        else if (lastHoveredObject != null)
        {
            Debug.Log("Mouse over: nothing");
            lastHoveredObject = null;
        }
    }
}
