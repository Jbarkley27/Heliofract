using UnityEngine;
using UnityEngine.UI;

public class ActivityFocusButton : MonoBehaviour
{
    [SerializeField] private Activity activity;
    [SerializeField] private OverviewCameraController overviewCameraController;
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;

    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (overviewCameraController == null)
        {
            overviewCameraController = FindFirstObjectByType<OverviewCameraController>();
        }

        ApplyIcon();
    }

    private void OnEnable()
    {
        if (button != null)
        {
            button.onClick.AddListener(FocusActivity);
        }
    }

    private void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(FocusActivity);
        }
    }

    public void FocusActivity()
    {
        if (overviewCameraController == null || activity == null)
        {
            return;
        }

        overviewCameraController.FocusActivity(activity);
    }

    private void ApplyIcon()
    {
        if (iconImage == null || activity == null)
        {
            return;
        }

        if (activity is PlanetActivity planetActivity && planetActivity.Definition != null)
        {
            iconImage.sprite = planetActivity.Definition.Icon;
            iconImage.enabled = planetActivity.Definition.Icon != null;
        }
    }
}
