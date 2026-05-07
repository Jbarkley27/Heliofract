using TMPro;
using UnityEngine;

public class MiningView : MonoBehaviour
{
    [SerializeField] private GameObject miningRoot;
    [SerializeField] private TMP_Text planetNameText;
    [SerializeField] private MiningGridView miningGridView;

    private PlanetActivity currentPlanet;
    private PlanetMiningState currentMiningState;

    private void Awake()
    {
        Close();
    }

    public void Open(PlanetActivity planet)
    {
        currentPlanet = planet;

        if (planetNameText != null)
        {
            planetNameText.text = GetPlanetName(planet);
        }

        currentMiningState = CreateMiningState(planet);

        if (miningRoot != null)
        {
            miningRoot.SetActive(true);
        }

        if (miningGridView != null)
        {
            miningGridView.Show(currentMiningState);
        }
    }

    public void Close()
    {
        currentPlanet = null;
        currentMiningState = null;

        if (miningGridView != null)
        {
            miningGridView.Clear();
        }

        if (miningRoot != null)
        {
            miningRoot.SetActive(false);
        }
    }

    private PlanetMiningState CreateMiningState(PlanetActivity planet)
    {
        if (planet == null || planet.Definition == null)
        {
            return null;
        }

        return MiningStateFactory.CreateInitialState(planet.Definition);
    }

    private string GetPlanetName(PlanetActivity planet)
    {
        if (planet == null || planet.Definition == null)
        {
            return string.Empty;
        }

        return planet.Definition.DisplayName;
    }
}
