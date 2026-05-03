using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceListUI : MonoBehaviour
{
    [SerializeField] private GameState gameState;
    [SerializeField] private ResourceInventory resourceInventory;
    [SerializeField] private Transform resourceParent;
    [SerializeField] private ResourceDatabase resourceDatabase;
    [SerializeField] private NumberFormatMode numberFormatMode = NumberFormatMode.Incremental;

    private void OnEnable()
    {
        if (gameState != null)
        {
            gameState.StateChanged += Refresh;
        }

        if (resourceInventory != null)
        {
            resourceInventory.ResourceAmountsChanged += Refresh;
        }
    }

    private void Start()
    {
        Refresh();
    }

    private void OnDisable()
    {
        if (gameState != null)
        {
            gameState.StateChanged -= Refresh;
        }

        if (resourceInventory != null)
        {
            resourceInventory.ResourceAmountsChanged -= Refresh;
        }
    }

    public void Refresh()
    {
        if (gameState == null || resourceInventory == null || resourceParent == null || resourceDatabase == null)
        {
            return;
        }

        DisableAllResourceSlots();

        int slotIndex = 0;

        foreach (ResourceType resourceType in gameState.DiscoveredResources)
        {
            ResourceDefinition definition = resourceDatabase.GetDefinition(resourceType);

            if (definition == null)
            {
                Debug.LogWarning($"Missing ResourceDefinition for {resourceType}.", this);
                continue;
            }

            if (slotIndex >= resourceParent.childCount)
            {
                Debug.LogWarning($"Not enough resource UI slots to show {resourceType}.", this);
                return;
            }

            Transform slot = resourceParent.GetChild(slotIndex);
            slot.gameObject.SetActive(true);

            double amount = resourceInventory.GetAmount(resourceType);
            ResourceListItemUI listItem = slot.GetComponent<ResourceListItemUI>();

            if (listItem != null)
            {
                listItem.SetResource(definition, amount, numberFormatMode);
            }
            else
            {
                Image icon = slot.GetComponentInChildren<Image>(true);
                TMP_Text amountText = slot.GetComponentInChildren<TMP_Text>(true);

                if (icon != null)
                {
                    icon.sprite = definition.Icon;
                    icon.enabled = definition.Icon != null;
                }

                if (amountText != null)
                {
                    amountText.text = NumberFormatter.FormatNumber(amount, numberFormatMode);
                }
            }

            slotIndex++;
        }
    }

    private void DisableAllResourceSlots()
    {
        for (int i = 0; i < resourceParent.childCount; i++)
        {
            resourceParent.GetChild(i).gameObject.SetActive(false);
        }
    }

}
