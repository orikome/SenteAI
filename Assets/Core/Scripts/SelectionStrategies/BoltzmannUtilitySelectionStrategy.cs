using System.Linq;
using UnityEngine;

namespace SenteAI.Core
{
    [CreateAssetMenu(
        fileName = "BoltzmannUtilitySelectionStrategy",
        menuName = "SenteAI/SelectionStrategies/BoltzmannUtilitySelection"
    )]
    public class BoltzmannUtilitySelectionStrategy : ActionSelectionStrategy
    {
        [SerializeField, Range(0.01f, 1.0f)]
        [Tooltip(
            "Temperature parameter to control how much randomness the agent uses when selecting actions."
        )]
        private float _temperature = 1.0f;

        public override AgentAction SelectAction(Agent agent)
        {
            UnityEngine.Profiling.Profiler.BeginSample("Boltzmann Utility Selection Strategy");

            // Step 1: Calculate the utility score for each action
            foreach (var action in agent.Actions)
            {
                action.CalculateUtility();
            }

            // Step 2: Adjust the utility score for each action and add them up
            var validActions = agent.Actions.Where(action => action.CanExecute()).ToList();

            if (validActions.Count == 0)
            {
                return null;
            }

            // Calculate a new value for each action based on the utility score and temperature
            float totalScore = 0.0f;
            foreach (var action in validActions)
            {
                // Using a simplified Boltzmann function to adjust the score: p = e^(S / T) - 1
                action.ScaledUtilityScore = Mathf.Max(
                    Mathf.Exp(action.ScaledUtilityScore / _temperature) - 1,
                    0.0f
                );
                totalScore += action.ScaledUtilityScore;
            }

            if (totalScore < Utility.MIN_UTILITY)
            {
                AgentLogger.LogWarning($"No valid actions available for agent: {agent.name}");
                return null;
            }

            // Step 3: Pick a random value to help choose an action - exploration
            float randomValue = Random.value;
            float runningTotal = 0.0f;

            // Step 4: Add up each action's score until we pass the random value
            foreach (var action in validActions)
            {
                runningTotal += action.ScaledUtilityScore / totalScore;
                if (randomValue <= runningTotal)
                {
                    AgentLogger.Log(
                        $"Selected: {Helpers.Bold(Helpers.CleanName(action.name))} - Score: {action.ScaledUtilityScore}",
                        agent.gameObject
                    );

                    agent.Metrics.AddActionToHistory(action);

                    return action;
                }
            }

            UnityEngine.Profiling.Profiler.EndSample();

            // Fallback in case no action is selected
            AgentLogger.LogWarning($"Fallback - No action selected for agent: {agent.name}");
            return null;
        }
    }
}
