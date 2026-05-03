using TMPro;
using UnityEngine;

public class MiningView : MonoBehaviour
{
    [SerializeField] private GameObject miningRoot;
    [SerializeField] private TMP_Text planetNameText;

    private PlanetActivity currentPlanet;

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

        if (miningRoot != null)
        {
            miningRoot.SetActive(true);
        }
    }

    public void Close()
    {
        currentPlanet = null;

        if (miningRoot != null)
        {
            miningRoot.SetActive(false);
        }
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
