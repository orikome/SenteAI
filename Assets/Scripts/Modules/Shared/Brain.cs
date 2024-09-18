using UnityEngine;

[CreateAssetMenu(fileName = "Brain", menuName = "Module/Brain")]
public class Brain : Module
{
    private Agent _agent;
    private ActionSelectionStrategy _actionSelectionStrategy;
    private CooldownHandler _cooldownHandler;

    public override void Initialize(Agent agent)
    {
        _agent = agent;
        _cooldownHandler = new CooldownHandler(agent.Data.actionCooldown);
        _actionSelectionStrategy = agent.Data.actionSelectionStrategy;

        if (_actionSelectionStrategy == null)
        {
            DebugManager.Instance.LogError("ActionSelectionStrategy is not assigned in AgentData!");
        }
        ResetUtilityScores();
    }

    public override void Execute(Agent agent)
    {
        if (_cooldownHandler.IsReady())
        {
            AgentAction decidedAction = _actionSelectionStrategy.SelectAction(agent);

            if (decidedAction != null)
            {
                decidedAction.Execute(
                    agent.firePoint,
                    _actionSelectionStrategy.GetShootDirection(agent)
                );
                _cooldownHandler.Reset();
            }
            else
            {
                DebugManager.Instance.LogWarning("No valid action selected.");
            }
        }
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
