using System.Linq;
using UnityEngine;

[CreateAssetMenu(
    fileName = "BiasWeightSelectionStrategy",
    menuName = "SenteAI/SelectionStrategies/BiasWeightSelection"
)]
public class BiasWeightSelectionStrategy : ActionSelectionStrategy
{
    public override AgentAction SelectAction(Agent agent)
    {
        foreach (var action in agent.Actions)
        {
            action.RestorePenaltyAndFeedback();
        }

        AgentAction selectedAction = agent
            .Actions.Where(action => action.CanExecute())
            .OrderByDescending(action => action.biasWeight)
            .FirstOrDefault();

        if (selectedAction != null)
        {
            AgentLogger.Log(
                $"Selected: {Helpers.CleanName(selectedAction.name)} with biasWeight: {selectedAction.biasWeight}"
            );
        }

        agent.Metrics.AddActionToHistory(selectedAction);

        return selectedAction;
    }
}
