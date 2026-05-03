using UnityEngine;


public abstract class Activity : MonoBehaviour
{
    public ActivityState State;

    public abstract ActivityType Type { get; }

    public virtual bool CanInteract()
    {
        return State == ActivityState.Unlocked || State == ActivityState.Active;
    }

    public abstract void Interact();
}


public enum ActivityState
{
    Hidden,
    Locked,
    Unlocked,
    Active
}


public enum ActivityType
{
    Planet
}




