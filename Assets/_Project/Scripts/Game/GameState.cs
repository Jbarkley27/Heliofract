using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public event System.Action StateChanged;

    public List<string> UnlockedActivityIds = new List<string>();
    public List<ResourceType> DiscoveredResources = new List<ResourceType>();

    public void UnlockPlanet(PlanetDefinition planet)
    {
        if (planet == null)
        {
            return;
        }

        AddUnlockedActivity(planet.Id);

        foreach (ResourceType resource in planet.AvailableResources)
        {
            AddDiscoveredResource(resource);
        }
    }

    public bool IsPlanetUnlocked(PlanetDefinition planet)
    {
        return planet != null && UnlockedActivityIds.Contains(planet.Id);
    }

    public bool IsResourceDiscovered(ResourceType resource)
    {
        return DiscoveredResources.Contains(resource);
    }

    private void AddUnlockedActivity(string activityId)
    {
        if (string.IsNullOrWhiteSpace(activityId) || UnlockedActivityIds.Contains(activityId))
        {
            return;
        }

        UnlockedActivityIds.Add(activityId);
        StateChanged?.Invoke();
    }

    private void AddDiscoveredResource(ResourceType resource)
    {
        if (DiscoveredResources.Contains(resource))
        {
            return;
        }

        DiscoveredResources.Add(resource);
        StateChanged?.Invoke();
    }
}
