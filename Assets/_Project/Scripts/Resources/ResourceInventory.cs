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
        SetAmountInternal(resourceType, amount, true);
    }

    public void AddAmount(ResourceType resourceType, double amount)
    {
        if (amount == 0)
        {
            return;
        }

        SetAmountInternal(resourceType, GetAmount(resourceType) + amount, true);
    }

    public void AddAmounts(IReadOnlyList<ResourceAmount> amounts)
    {
        if (amounts == null || amounts.Count == 0)
        {
            return;
        }

        bool changed = false;

        for (int i = 0; i < amounts.Count; i++)
        {
            ResourceAmount amount = amounts[i];

            if (amount.Amount == 0)
            {
                continue;
            }

            SetAmountInternal(amount.Type, GetAmount(amount.Type) + amount.Amount, false);
            changed = true;
        }

        if (changed)
        {
            ResourceAmountsChanged?.Invoke();
        }
    }

    private void SetAmountInternal(ResourceType resourceType, double amount, bool notifyChanged)
    {
        amount = Math.Max(0, amount);

        for (int i = 0; i < resourceAmounts.Count; i++)
        {
            if (resourceAmounts[i].Type != resourceType)
            {
                continue;
            }

            if (resourceAmounts[i].Amount == amount)
            {
                return;
            }

            resourceAmounts[i] = new ResourceAmount(resourceType, amount);

            if (notifyChanged)
            {
                ResourceAmountsChanged?.Invoke();
            }

            return;
        }

        resourceAmounts.Add(new ResourceAmount(resourceType, amount));

        if (notifyChanged)
        {
            ResourceAmountsChanged?.Invoke();
        }
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
