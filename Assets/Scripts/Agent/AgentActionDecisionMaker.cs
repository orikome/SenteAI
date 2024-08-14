using UnityEngine;

public class AgentActionDecisionMaker : MonoBehaviour
{
    private Agent agent;

    public void Initialize(Agent agent)
    {
        this.agent = agent;
    }

    public AgentAction MakeDecision()
    {
        var actionToUse = agent.actionSelectionStrategy.SelectAction(agent);

        if (agent.readinessModule.CanPerformAction(actionToUse))
        {
            agent.readinessModule.OnActionPerformed(actionToUse);

            DebugLog(actionToUse);

            return actionToUse;
        }

        return null;
    }

    private void DebugLog(AgentAction actionToUse)
    {
        string actionName = actionToUse.name.Replace("(Clone)", "").Trim();

        float weight = 0f;

        if (agent.actionWeightManager.weights.TryGetValue(actionToUse, out float foundWeight))
        {
            weight = foundWeight;
        }

        DebugManager.Instance.Log(
            transform,
            $"{actionName} C:{actionToUse.cost} W:{weight:F2}",
            Color.cyan
        );
    }

    public void SetActionSelectionStrategy(ActionSelectionStrategy strategy)
    {
        // If you need to change the strategy during runtime for some reason
        agent.actionSelectionStrategy = strategy;
    }
}
