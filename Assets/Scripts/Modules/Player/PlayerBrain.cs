using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBrain", menuName = "Module/PlayerBrain")]
public class PlayerBrain : Module
{
    private Agent _agent;
    private PlayerInputSelectionStrategy _playerInputSelection;
    private readonly float _globalCooldown = 0.2f;
    private float _shootingTimer;

    public override void Initialize(Agent agent)
    {
        _agent = agent;

        // Initialize the player input selection strategy from AgentData
        _playerInputSelection = (PlayerInputSelectionStrategy)_agent.Data.actionSelectionStrategy;
        if (_playerInputSelection == null)
        {
            DebugManager.Instance.LogError(
                "PlayerInputSelectionStrategy is not assigned in AgentData!"
            );
        }
    }

    public override void Execute(Agent agent)
    {
        _shootingTimer -= Time.deltaTime;

        if (_shootingTimer <= 0f && _playerInputSelection.IsInputHeld())
        {
            ExecutePlayerAction();
            _shootingTimer = _globalCooldown;
        }
    }

    private void ExecutePlayerAction()
    {
        AgentAction decidedAction = _playerInputSelection.SelectAction(_agent);
        if (decidedAction == null)
        {
            DebugManager.Instance.LogWarning("No valid action selected.");
            return;
        }

        decidedAction.Execute(_agent.firePoint, GetPlayerShootDirection());
    }

    private Vector3 GetPlayerShootDirection()
    {
        var nearestEnemy = Player.Instance.Metrics.FindClosestEnemyToPlayer();
        if (nearestEnemy != null)
        {
            return (nearestEnemy.position - _agent.firePoint.position).normalized;
        }

        return _agent.firePoint.forward;
    }
}
