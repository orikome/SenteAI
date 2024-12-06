using System.Linq;
using UnityEngine;

[CreateAssetMenu(
    fileName = "MaxUtilitySelectionStrategy",
    menuName = "ActionSelection/MaxUtilitySelection"
)]
public class MaxUtilitySelectionStrategy : ActionSelectionStrategy
{
    public override AgentAction SelectAction(Agent agent)
    {
        // Calculate utility scores
        foreach (var action in agent.Actions)
        {
            action.CalculateUtility(agent);
        }

        // Normalize and select the best action
        //NormalizeUtilityScores(agent);

        // Select action
        AgentAction selectedAction = agent
            .Actions.Where(action => action.CanExecute(agent))
            .OrderByDescending(action => action.ScaledUtilityScore)
            .FirstOrDefault();

        if (selectedAction != null)
            DebugManager.Instance.Log(
                $"Selected: {Helpers.CleanName(selectedAction.name)} with utilScore: {selectedAction.ScaledUtilityScore}"
            );

        agent.Metrics.AddActionToHistory(selectedAction);

        return selectedAction;
    }

    private void NormalizeUtilityScores(Agent agent)
    {
        float sum = agent.Actions.Sum(action => action.ScaledUtilityScore);
        if (sum == 0)
            return;

        foreach (var action in agent.Actions)
        {
            action.ScaledUtilityScore /= sum;
        }
    }
}
