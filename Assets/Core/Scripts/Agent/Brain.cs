using UnityEngine;

[CreateAssetMenu(fileName = "Brain", menuName = "SenteAI/Modules/Brain")]
public class Brain : Module
{
    public ActionSelectionStrategy ActionSelectionStrategy { get; private set; }
    public AgentAction CurrentAction { get; private set; }
    private CooldownHandler _cooldownHandler;

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
            CurrentAction = ActionSelectionStrategy.SelectAction(_agent);

        if (!_cooldownHandler.IsReady())
            return;

        CurrentAction = ActionSelectionStrategy.SelectAction(_agent);

        if (CurrentAction != null)
        {
            CurrentAction.Execute(
                _agent.firePoint,
                ActionSelectionStrategy.GetShootDirection(_agent)
            );
            _cooldownHandler.Reset();
        }
    }

    public void SetAction(AgentAction action)
    {
        CurrentAction = action;
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
    }

    public void PauseFor(float duration)
    {
        _cooldownHandler.PauseFor(duration);
    }
}
