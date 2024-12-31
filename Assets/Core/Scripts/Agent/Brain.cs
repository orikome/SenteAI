using UnityEngine;

[CreateAssetMenu(fileName = "Brain", menuName = "SenteAI/Modules/Brain")]
public class Brain : Module
{
    public ActionSelectionStrategy ActionSelectionStrategy { get; private set; }
    private CooldownHandler _cooldownHandler;
    private AgentAction _currentAction;

    public override void Initialize(Agent agent)
    {
        base.Initialize(agent);
        _cooldownHandler = new CooldownHandler(agent.data.actionCooldown);
        _cooldownHandler.Reset();
        ActionSelectionStrategy = agent.data.actionSelectionStrategy;

        if (ActionSelectionStrategy == null)
        {
            AgentLogger.LogError("ActionSelectionStrategy is not assigned in AgentData!");
        }
        ResetUtilityScores();
    }

    public override void Execute()
    {
        if (_agent.Faction == Faction.Player)
            _currentAction = ActionSelectionStrategy.SelectAction(_agent);

        if (!_cooldownHandler.IsReady())
            return;

        _currentAction = ActionSelectionStrategy.SelectAction(_agent);

        if (_currentAction != null)
        {
            _currentAction.Execute(
                _agent.firePoint,
                ActionSelectionStrategy.GetShootDirection(_agent)
            );
            _cooldownHandler.Reset();
        }
    }

    public void SetActionSelectionStrategy(ActionSelectionStrategy strategy)
    {
        ActionSelectionStrategy = strategy;
    }

    private void ResetUtilityScores()
    {
        foreach (AgentAction action in _agent.Actions)
        {
            action.ScaledUtilityScore = 1.0f / _agent.Actions.Count;
        }
        //AgentLogger.Log("Reset utilScores", _agent.gameObject);
    }

    public void PauseFor(float duration)
    {
        _cooldownHandler.PauseFor(duration);
    }
}
