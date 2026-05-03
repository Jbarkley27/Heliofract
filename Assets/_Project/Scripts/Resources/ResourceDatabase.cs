using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Heliofract/Resource Database")]
public class ResourceDatabase : ScriptableObject
{
    [SerializeField] private List<ResourceDefinition> resources = new List<ResourceDefinition>();

    public IReadOnlyList<ResourceDefinition> Resources => resources;

    public ResourceDefinition GetDefinition(ResourceType resourceType)
    {
        for (int i = 0; i < resources.Count; i++)
        {
            ResourceDefinition definition = resources[i];

            if (definition != null && definition.Type == resourceType)
            {
                return definition;
            }
        }

        return null;
    }
}
