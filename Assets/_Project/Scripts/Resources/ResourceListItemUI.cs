using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceListItemUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text amountText;

    public void SetResource(ResourceDefinition definition, double amount, NumberFormatMode formatMode)
    {
        if (icon != null)
        {
            icon.sprite = definition.Icon;
            icon.enabled = definition.Icon != null;
        }

        if (amountText != null)
        {
            amountText.text = NumberFormatter.FormatNumber(amount, formatMode);
        }
    }
}
