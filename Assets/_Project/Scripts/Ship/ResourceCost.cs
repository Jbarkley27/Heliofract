using System;
using UnityEngine;

[Serializable]
public struct ResourceCost
{
    public ResourceType Type;

    [Min(0)]
    public double Amount;

    public ResourceCost(ResourceType type, double amount)
    {
        Type = type;
        Amount = amount;
    }
}
