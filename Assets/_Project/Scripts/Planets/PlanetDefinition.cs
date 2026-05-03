using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Heliofract/Planet Definition")]
public class PlanetDefinition : ScriptableObject
{
    public string Id;
    public string DisplayName;
    public Sprite Icon;

    public float SurfaceIntegrity;

    public List<ResourceType> AvailableResources;
    public bool InitiallyUnlocked;
}
