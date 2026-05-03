using UnityEngine;

public class PlanetActivity : Activity
{
    public PlanetDefinition Definition;
    [SerializeField] private MiningView miningView;

    public override ActivityType Type => ActivityType.Planet;

    private void Awake()
    {
        if (miningView == null)
        {
            miningView = FindFirstObjectByType<MiningView>(FindObjectsInactive.Include);
        }
    }

    public override void Interact()
    {
        if (!CanInteract())
        {
            return;
        }

        if (miningView == null)
        {
            Debug.LogWarning($"No MiningView found for {name}.", this);
            return;
        }

        miningView.Open(this);
    }
}
