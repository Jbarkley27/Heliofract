using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIPointerDebugger : MonoBehaviour
{
    [SerializeField] private bool logOnLeftClick = true;
    [SerializeField] private bool logOnRightClick = true;
    [SerializeField] private bool logWhenHoverTargetChanges;

    private readonly List<RaycastResult> raycastResults = new List<RaycastResult>();
    private GameObject lastTopHoverTarget;

    private void Update()
    {
        if (EventSystem.current == null || Mouse.current == null)
        {
            return;
        }

        if (logWhenHoverTargetChanges)
        {
            LogHoverChange();
        }

        if (logOnLeftClick && Mouse.current.leftButton.wasPressedThisFrame)
        {
            LogPointerStack("Left click");
        }

        if (logOnRightClick && Mouse.current.rightButton.wasPressedThisFrame)
        {
            LogPointerStack("Right click");
        }
    }

    private void LogHoverChange()
    {
        RaycastAll();

        GameObject topTarget = raycastResults.Count > 0
            ? raycastResults[0].gameObject
            : null;

        if (topTarget == lastTopHoverTarget)
        {
            return;
        }

        lastTopHoverTarget = topTarget;

        if (topTarget == null)
        {
            Debug.Log("UI hover: nothing", this);
            return;
        }

        Debug.Log($"UI hover top: {GetObjectPath(topTarget)}", topTarget);
    }

    private void LogPointerStack(string reason)
    {
        RaycastAll();

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        if (raycastResults.Count == 0)
        {
            Debug.Log($"{reason} at {mousePosition}: no UI raycast hits.", this);
            return;
        }

        string message = $"{reason} at {mousePosition}: {raycastResults.Count} UI raycast hit(s)\n";

        for (int i = 0; i < raycastResults.Count; i++)
        {
            RaycastResult result = raycastResults[i];
            GameObject hitObject = result.gameObject;

            message +=
                $"{i + 1}. {GetObjectPath(hitObject)}" +
                $" | RaycastTarget: {GetRaycastTargetText(hitObject)}" +
                $" | Button: {GetButtonText(hitObject)}" +
                $" | CanvasGroup: {GetCanvasGroupText(hitObject)}\n";
        }

        Debug.Log(message, raycastResults[0].gameObject);
    }

    private void RaycastAll()
    {
        raycastResults.Clear();

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };

        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
    }

    private string GetRaycastTargetText(GameObject target)
    {
        Graphic graphic = target.GetComponent<Graphic>();

        if (graphic == null)
        {
            return "No Graphic";
        }

        return graphic.raycastTarget ? "On" : "Off";
    }

    private string GetButtonText(GameObject target)
    {
        Button button = target.GetComponentInParent<Button>();

        if (button == null)
        {
            return "None";
        }

        return $"{GetObjectPath(button.gameObject)} Interactable:{button.interactable}";
    }

    private string GetCanvasGroupText(GameObject target)
    {
        CanvasGroup[] canvasGroups = target.GetComponentsInParent<CanvasGroup>();

        if (canvasGroups.Length == 0)
        {
            return "None";
        }

        string text = string.Empty;

        for (int i = 0; i < canvasGroups.Length; i++)
        {
            CanvasGroup canvasGroup = canvasGroups[i];

            if (i > 0)
            {
                text += " -> ";
            }

            text +=
                $"{canvasGroup.name}" +
                $" Alpha:{canvasGroup.alpha:0.##}" +
                $" Interactable:{canvasGroup.interactable}" +
                $" Blocks:{canvasGroup.blocksRaycasts}" +
                $" IgnoreParent:{canvasGroup.ignoreParentGroups}";
        }

        return text;
    }

    private string GetObjectPath(GameObject target)
    {
        if (target == null)
        {
            return "None";
        }

        string path = target.name;
        Transform current = target.transform.parent;

        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }

        return path;
    }
}
