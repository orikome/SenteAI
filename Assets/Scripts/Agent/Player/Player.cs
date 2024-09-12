using UnityEngine;

public class Player : Agent
{
    public static Player Instance { get; private set; }
    public PlayerMetrics Metrics { get; private set; }
    public PlayerInputSelectionStrategy PlayerInputSelection { get; private set; }
    private readonly float _globalCooldown = 0.2f;
    private float _shootingTimer;

    void Awake()
    {
        Instance = this;
        Metrics = gameObject.GetComponent<PlayerMetrics>();
        LoadAgentData();
        PlayerInputSelection = (PlayerInputSelectionStrategy)Data.actionSelectionStrategy;
        InitModules();
        InitActions();
    }

    public override void Update()
    {
        foreach (var module in Modules)
        {
            module.Execute(null);
        }

        _shootingTimer -= Time.deltaTime;

        if (_shootingTimer <= 0f && PlayerInputSelection.IsInputHeld())
        {
            ExecutePlayerAction();
            _shootingTimer = _globalCooldown;
        }
    }

    private void ExecutePlayerAction()
    {
        AgentAction decidedAction = PlayerInputSelection.SelectAction(this);
        if (decidedAction == null)
        {
            DebugManager.Instance.LogWarning("No valid action selected.");
            return;
        }

        decidedAction.Execute(firePoint);
    }
}
