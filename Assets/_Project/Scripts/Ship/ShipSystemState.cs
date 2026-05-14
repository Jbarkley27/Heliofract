using System;

[Serializable]
public class ShipSystemState
{
    public string SystemId;
    public bool IsRevealed;
    public bool IsActivated;
    public int CurrentTier;

    public ShipSystemState(string systemId, bool isRevealed)
    {
        SystemId = systemId;
        IsRevealed = isRevealed;
        IsActivated = false;
        CurrentTier = 0;
    }

    public bool CanUpgrade(ShipSystemDefinition definition)
    {
        if (definition == null)
        {
            return false;
        }

        return CurrentTier < definition.Tiers.Count;
    }

    public int GetNextTierNumber()
    {
        return CurrentTier + 1;
    }
}
