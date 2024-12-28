using System.Linq;
using UnityEngine;

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
        // Step 1: Calculate the utility score for each action
        foreach (var action in agent.Actions)
        {
            action.CalculateUtility(agent);
        }

        // Step 2: Adjust the utility score for each action and add them up
        float totalScore = 0.0f;
        var validActions = agent.Actions.Where(action => action.CanExecute(agent)).ToList();

        if (validActions.Count == 0)
        {
            //AgentLogger.LogWarning($"No valid actions available for agent: {agent.name}");
            return null;
        }

        // Calculate a new value for each action based on the utility score and temperature
        foreach (var action in validActions)
        {
            // Using a simplified Boltzmann function to adjust the score: p = e^(S / T) - 1
            action.ScaledUtilityScore = Mathf.Max(
                Mathf.Exp(action.ScaledUtilityScore / _temperature) - 1,
                0.0f
            );
            totalScore += action.ScaledUtilityScore;
        }

        if (totalScore == 0.0f)
        {
            AgentLogger.LogWarning($"No valid actions available for agent: {agent.name}");
            return null;
        }

        // Step 3: Pick a random value to help choose an action
        float randomValue = Random.value;
        float runningTotal = 0.0f;

        // Step 4: Add up each action's score until we pass the random value
        foreach (var action in validActions)
        {
            runningTotal += action.ScaledUtilityScore / totalScore;
            if (randomValue <= runningTotal)
            {
                AgentLogger.Log(
                    $"Selected: {Helpers.CleanName(action.name)} with utility score: {action.ScaledUtilityScore}",
                    agent.gameObject
                );

                agent.Metrics.AddActionToHistory(action);

                return action;
            }
        }

        // Fallback in case no action is selected (due to rounding issues)
        AgentLogger.LogWarning($"Fallback - No action selected for agent: {agent.name}");
        return validActions.Last();
    }
}
