using UnityEngine;
using UnityEngine.Serialization;


public abstract class Activity : MonoBehaviour
{
    public event System.Action<ActivityState> StateChanged;

    [FormerlySerializedAs("State")]
    [SerializeField] private ActivityState state;

    public ActivityState State
    {
        get => state;
        set
        {
            if (state == value)
            {
                return;
            }

            state = value;
            StateChanged?.Invoke(state);
        }
    }

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


