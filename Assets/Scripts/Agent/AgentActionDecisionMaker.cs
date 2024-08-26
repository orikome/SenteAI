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
        float bestUtility = -1f;
        AgentAction bestAction = null;

        foreach (var action in _agent.ActionUtilityManager.actions)
        {
            if (action.CanExecute(_agent))
            {
                float utility = action.CalculateUtility(_agent, _agent.AgentMetrics);

                if (utility > bestUtility)
                {
                    bestUtility = utility;
                    bestAction = action;
                }
            }
        }
        DebugLog(bestAction);
        return bestAction;
    }

    private void DebugLog(AgentAction actionToUse)
    {
        string actionName = Helpers.CleanName(actionToUse.name);

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
