using UnityEngine;

[CreateAssetMenu(fileName = "Brain", menuName = "Module/Brain")]
public class Brain : Module
{
    private Agent _agent;
    public ActionSelectionStrategy ActionSelectionStrategy { get; private set; }
    private CooldownHandler _cooldownHandler;

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
        if (_cooldownHandler.IsReady())
        {
            AgentAction decidedAction = ActionSelectionStrategy.SelectAction(agent);

            if (decidedAction != null)
            {
                decidedAction.Execute(
                    agent.firePoint,
                    ActionSelectionStrategy.GetShootDirection(agent)
                );
                _cooldownHandler.Reset();
            }
            else
            {
                //DebugManager.Instance.LogWarning("No valid action selected.");
            }
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
