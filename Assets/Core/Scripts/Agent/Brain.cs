using UnityEngine;

[CreateAssetMenu(fileName = "Brain", menuName = "SenteAI/Modules/Brain")]
public class Brain : Module
{
    public ActionSelectionStrategy ActionSelectionStrategy { get; private set; }
    private Agent _agent;
    private CooldownHandler _cooldownHandler;
    private AgentAction _currentAction;

    public override void Initialize(Agent agent)
    {
        _agent = agent;
        _cooldownHandler = new CooldownHandler(agent.Data.actionCooldown);
        ActionSelectionStrategy = agent.Data.actionSelectionStrategy;

        if (ActionSelectionStrategy == null)
        {
            DebugManager.Instance.LogError("ActionSelectionStrategy is not assigned in AgentData!");
        }
        ResetUtilityScores();
    }

    public override void Execute(Agent agent)
    {
        if (!_cooldownHandler.IsReady())
            return;

        _currentAction = ActionSelectionStrategy.SelectAction(agent);

        if (_currentAction != null)
        {
            _currentAction.Execute(
                agent.firePoint,
                ActionSelectionStrategy.GetShootDirection(agent)
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
        DebugManager.Instance.SpawnTextLog(_agent.transform, "Reset utilScores", Color.red);
    }
}
