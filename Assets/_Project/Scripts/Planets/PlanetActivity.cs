using UnityEngine;

public class PlanetActivity : Activity
{
    public PlanetDefinition Definition;

    public override ActivityType Type => ActivityType.Planet;

    public override void Interact()
    {
        if (!CanInteract())
        {
            return;
        }

        // Open mining grid / focus camera / select planet.
    }
}
