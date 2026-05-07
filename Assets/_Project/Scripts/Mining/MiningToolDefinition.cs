using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Heliofract/Mining/Mining Tool Definition")]
public class MiningToolDefinition : ScriptableObject
{
    [Header("Damage")]
    [Min(0)]
    public double BaseDamage = 1;

    [Header("Critical Hit")]
    [Range(0f, 1f)]
    public float CritChance = 0.05f;

    [Min(1)]
    public double CritDamageMultiplier = 2;

    [Min(1)]
    public double CritRewardMultiplier = 2;

    [Header("Behavior")]
    public bool CarryoverDamageEnabled;

    [Min(0.01f)]
    public float HoldInterval = 0.15f;

    [Header("Pattern")]
    public List<MiningToolPatternOffset> PatternOffsets = new List<MiningToolPatternOffset>
    {
        new MiningToolPatternOffset(Vector2Int.zero, 1)
    };

    public IReadOnlyList<MiningToolPatternOffset> GetPattern()
    {
        return PatternOffsets;
    }
}
