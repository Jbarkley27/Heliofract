using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetResourceIconsUI : MonoBehaviour
{
    [SerializeField] private PlanetActivity planetActivity;
    [SerializeField] private ResourceDatabase resourceDatabase;
    [SerializeField] private Image[] resourceIconSlots;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        DisableAllSlots();

        if (planetActivity == null || planetActivity.Definition == null || resourceDatabase == null)
        {
            return;
        }

        List<ResourceType> resources = planetActivity.Definition.AvailableResources;

        for (int i = 0; i < resources.Count && i < resourceIconSlots.Length; i++)
        {
            ResourceDefinition definition = resourceDatabase.GetDefinition(resources[i]);

            if (definition == null || resourceIconSlots[i] == null)
            {
                continue;
            }

            resourceIconSlots[i].sprite = definition.Icon;
            resourceIconSlots[i].gameObject.SetActive(definition.Icon != null);
        }
    }

    private void DisableAllSlots()
    {
        for (int i = 0; i < resourceIconSlots.Length; i++)
        {
            if (resourceIconSlots[i] != null)
            {
                resourceIconSlots[i].gameObject.SetActive(false);
            }
        }
    }

}
