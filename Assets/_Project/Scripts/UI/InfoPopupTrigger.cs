using UnityEngine;
using UnityEngine.EventSystems;

public class InfoPopupTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private InfoPopupView infoPopup;
    [SerializeField] private Sprite icon;
    [SerializeField] private string title;
    [TextArea]
    [SerializeField] private string description;
    [TextArea]
    [SerializeField] private string extra;

    private bool isHovered;

    private void Awake()
    {
        if (infoPopup == null)
        {
            infoPopup = FindFirstObjectByType<InfoPopupView>(FindObjectsInactive.Include);
        }
    }

    private void OnDisable()
    {
        if (isHovered)
        {
            Hide();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        Show();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Hide();
    }

    public void SetData(InfoPopupData data)
    {
        icon = data.Icon;
        title = data.Title;
        description = data.Description;
        extra = data.Extra;

        if (isHovered)
        {
            Show();
        }
    }

    private void Show()
    {
        if (infoPopup == null)
        {
            return;
        }

        infoPopup.Show(new InfoPopupData(title, description, extra, icon));
    }

    private void Hide()
    {
        isHovered = false;
        infoPopup?.Hide();
    }
}
