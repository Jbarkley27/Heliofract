using UnityEngine;

public class PlanetActivity : Activity
{
    public event System.Action<PlanetMiningState> MiningStateChanged;

    public PlanetDefinition Definition;

    [Header("Mining")]
    [SerializeField] private MiningGridView miningGridView;
    [SerializeField] private PlanetMapDetailsView detailsView;

    public override ActivityType Type => ActivityType.Planet;

    private PlanetMiningState miningState;

    public PlanetMiningState MiningState => miningState;

    private void Awake()
    {

        if (detailsView == null)
        {
            detailsView = GetComponentInChildren<PlanetMapDetailsView>(true);
        }
    }

    private void OnEnable()
    {
        StateChanged += HandleActivityStateChanged;

        if (miningGridView != null)
        {
            miningGridView.MiningStateChanged += HandleMiningStateChanged;
        }
    }

    private void OnDisable()
    {
        StateChanged -= HandleActivityStateChanged;

        if (miningGridView != null)
        {
            miningGridView.MiningStateChanged -= HandleMiningStateChanged;
        }
    }

    private void Start()
    {
        if (CanInteract())
        {
            InitializeMiningState();
        }

        RefreshMiningGrid();
        detailsView?.Refresh();
    }

    public override void Interact()
    {
        if (!CanInteract())
        {
            return;
        }

        // The current map design keeps mining directly on the star map.
        // Tile clicks are handled by MiningGridView, so interacting with the planet
        // only confirms/selects the activity for now.
        Debug.Log($"Selected planet {GetPlanetDisplayName()}.", this);
    }

    public void InitializeMiningState()
    {
        if (!CanInteract())
        {
            return;
        }

        if (Definition == null)
        {
            Debug.LogWarning($"{name} cannot create mining state without a PlanetDefinition.", this);
            return;
        }

        if (miningState != null)
        {
            return;
        }

        miningState = MiningStateFactory.CreateInitialState(Definition);
        MiningStateChanged?.Invoke(miningState);
    }

    public void RefreshMiningGrid()
    {
        if (miningGridView == null)
        {
            return;
        }

        if (!CanInteract() || miningState == null)
        {
            miningGridView.Clear();
            return;
        }

        miningGridView.Show(miningState);
        detailsView?.Refresh();
    }

    private void HandleMiningStateChanged(PlanetMiningState changedState)
    {
        MiningStateChanged?.Invoke(changedState);
        detailsView?.Refresh();
    }

    private void HandleActivityStateChanged(ActivityState state)
    {
        if (CanInteract())
        {
            InitializeMiningState();
        }

        RefreshMiningGrid();
        detailsView?.Refresh();
    }

    private string GetPlanetDisplayName()
    {
        return Definition != null ? Definition.DisplayName : name;
    }
}
