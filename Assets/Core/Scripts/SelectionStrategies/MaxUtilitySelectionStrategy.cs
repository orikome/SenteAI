using System.Linq;
using UnityEngine;

namespace SenteAI.Core
{
    [CreateAssetMenu(
        fileName = "MaxUtilitySelectionStrategy",
        menuName = "SenteAI/SelectionStrategies/MaxUtilitySelection"
    )]
    public class MaxUtilitySelectionStrategy : ActionSelectionStrategy
    {
        public override AgentAction SelectAction(Agent agent)
        {
            // Calculate utility scores
            foreach (var action in agent.Actions)
            {
                action.CalculateUtility();
            }

            // Normalize and select the best action
            //NormalizeUtilityScores(agent);

            // Select action
            AgentAction selectedAction = agent
                .Actions.Where(action => action.CanExecute())
                .OrderByDescending(action => action.ScaledUtilityScore)
                .FirstOrDefault();

            if (selectedAction != null)
                AgentLogger.Log(
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
}
