using UnityEngine;

public class AgentActionDecisionMaker : MonoBehaviour
{
    private Agent agent;

    public void Initialize(Agent agent)
    {
        this.agent = agent;
    }

    // Called in the agent's update loop
    public AgentAction MakeDecision()
    {
        var actionToUse = agent.actionSelectionStrategy.SelectAction(agent);

        if (actionToUse != null && actionToUse.CanExecute(agent))
        {
            actionToUse.lastExecutedTime = Time.time;

            foreach (var action in agent.actionUtilityManager.actions)
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
        agent.actionSelectionStrategy = strategy;
    }
}
