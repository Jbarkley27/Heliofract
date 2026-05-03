using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceInventory : MonoBehaviour
{
    public event Action ResourceAmountsChanged;

    [SerializeField] private List<ResourceAmount> resourceAmounts = new List<ResourceAmount>();

    public IReadOnlyList<ResourceAmount> ResourceAmounts => resourceAmounts;

    public double GetAmount(ResourceType resourceType)
    {
        for (int i = 0; i < resourceAmounts.Count; i++)
        {
            if (resourceAmounts[i].Type == resourceType)
            {
                return resourceAmounts[i].Amount;
            }
        }

        return 0;
    }

    public void SetAmount(ResourceType resourceType, double amount)
    {
        amount = Math.Max(0, amount);

        for (int i = 0; i < resourceAmounts.Count; i++)
        {
            if (resourceAmounts[i].Type != resourceType)
            {
                continue;
            }

            resourceAmounts[i] = new ResourceAmount(resourceType, amount);
            ResourceAmountsChanged?.Invoke();
            return;
        }

        resourceAmounts.Add(new ResourceAmount(resourceType, amount));
        ResourceAmountsChanged?.Invoke();
    }

    public void AddAmount(ResourceType resourceType, double amount)
    {
        if (amount == 0)
        {
            return;
        }

        SetAmount(resourceType, GetAmount(resourceType) + amount);
    }
}

[Serializable]
public struct ResourceAmount
{
    public ResourceType Type;
    public double Amount;

    public ResourceAmount(ResourceType type, double amount)
    {
        Type = type;
        Amount = amount;
    }
}
