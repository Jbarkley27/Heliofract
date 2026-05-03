using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private GameState gameState;

    private void Awake()
    {
        InitializePlanets();
    }

    private void InitializePlanets()
    {
        // Find all PlanetActivity instances in the scene and set their initial state based on the GameState.
        PlanetActivity[] planets = FindObjectsByType<PlanetActivity>(FindObjectsSortMode.None);

        foreach (PlanetActivity planet in planets)
        {
            if (planet.Definition == null)
            {
                Debug.LogWarning($"{planet.name} is missing a PlanetDefinition.", planet);
                continue;
            }

            if (planet.Definition.InitiallyUnlocked)
            {
                gameState.UnlockPlanet(planet.Definition);
                planet.State = ActivityState.Unlocked;
            }
            else if (gameState.IsPlanetUnlocked(planet.Definition))
            {
                planet.State = ActivityState.Unlocked;
            }
            else
            {
                planet.State = ActivityState.Locked;
            }
        }
    }
}
