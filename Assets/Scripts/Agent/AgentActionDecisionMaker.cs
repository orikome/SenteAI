using UnityEngine;

public class AgentActionDecisionMaker : MonoBehaviour
{
    private Agent _agent;

    public void Initialize(Agent agent)
    {
        _agent = agent;
    }

    // Called in the agent's update loop
    public AgentAction MakeDecision()
    {
        var actionToUse = _agent.ActionSelectionStrategy.SelectAction(_agent);

        if (actionToUse != null && actionToUse.CanExecute(_agent))
        {
            actionToUse.lastExecutedTime = Time.time;

            foreach (var action in _agent.ActionUtilityManager.actions)
            {
                if (action != actionToUse)
                {
                    action.lastExecutedTime = Time.time;
                }
            }

            return actionToUse;
        }

        return null;
    }

    private void DebugLog(AgentAction actionToUse)
    {
        string actionName = actionToUse.name.Replace("(Clone)", "").Trim();

        float utilityScore = actionToUse.utilityScore;

        DebugManager.Instance.Log(
            transform,
            $"{actionName} C:{actionToUse.cost} W:{utilityScore:F2}",
            Color.cyan
        );
    }

    public void SetActionSelectionStrategy(ActionSelectionStrategy strategy)
    {
        // If you need to change the strategy during runtime for some reason
        //agent.actionSelectionStrategy = strategy;
    }
}
