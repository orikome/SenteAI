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
        //Debug.Log($"Agent X is executing action: {actionToUse.name}");
        DebugManager.Instance.Log(
            transform,
            $"{actionToUse.name} C:{actionToUse.cost}",
            Color.cyan
        );
    }

    public void SetActionSelectionStrategy(ActionSelectionStrategy strategy)
    {
        // If you need to change the strategy during runtime for some reason
        agent.actionSelectionStrategy = strategy;
    }
}
