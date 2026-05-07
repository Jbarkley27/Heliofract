using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Heliofract/Mining/Depth Database")]
public class DepthDatabase : ScriptableObject
{
    [SerializeField] private List<DepthDefinition> depths = new List<DepthDefinition>();

    public IReadOnlyList<DepthDefinition> Depths => depths;

    public bool TryGetDepth(int depthLevel, out DepthDefinition depthDefinition)
    {
        for (int i = 0; i < depths.Count; i++)
        {
            if (depths[i].Level == depthLevel)
            {
                depthDefinition = depths[i];
                return true;
            }
        }

        depthDefinition = default;
        return false;
    }

    public DepthDefinition GetDepthOrFallback(int depthLevel)
    {
        if (TryGetDepth(depthLevel, out DepthDefinition depthDefinition))
        {
            return depthDefinition;
        }

        return new DepthDefinition(
            depthLevel,
            $"Depth {depthLevel}",
            Color.white
        );
    }
}

[Serializable]
public struct DepthDefinition
{
    public int Level;
    public string DisplayName;
    public Color Color;

    public DepthDefinition(int level, string displayName, Color color)
    {
        Level = level;
        DisplayName = displayName;
        Color = color;
    }
}
