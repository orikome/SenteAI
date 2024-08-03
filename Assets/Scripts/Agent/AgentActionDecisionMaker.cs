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
            Debug.Log($"Executing action: {actionToUse.name}");
            return actionToUse;
        }

        return null;
    }

    public void SetActionSelectionStrategy(ActionSelectionStrategy strategy)
    {
        // If you need to change the strategy during runtime for some reason
        agent.actionSelectionStrategy = strategy;
    }
}
