using UnityEngine;

[CreateAssetMenu(menuName = "Heliofract/Resource Definition")]
public class ResourceDefinition : ScriptableObject
{
    public ResourceType Type;
    public string DisplayName;
    public Sprite Icon;
}
