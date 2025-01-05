using System.Linq;
using UnityEngine;

[CreateAssetMenu(
    fileName = "RouletteSelectionStrategy",
    menuName = "SenteAI/SelectionStrategies/RouletteSelectionStrategy"
)]
public class RouletteSelectionStrategy : ActionSelectionStrategy
{
    public override AgentAction SelectAction(Agent agent)
    {
        // Roulette wheel picker with sizes proportional to utility scores

        var executableActions = agent.Actions.Where(action => action.CanExecute()).ToList();

        if (executableActions.Count == 0)
        {
            AgentLogger.LogWarning("No executable actions available!");
            return null;
        }

        // Find the action with the highest utility
        var maxUtilityAction = executableActions
            .OrderByDescending(action => action.ScaledUtilityScore)
            .First();
        float total = executableActions.Sum(action => action.ScaledUtilityScore);
        float randomPoint = Random.value * total;

        AgentAction selectedAction = null;

        foreach (var action in executableActions)
        {
            if (randomPoint < action.ScaledUtilityScore)
            {
                selectedAction = action;
                break;
            }
            else
            {
                randomPoint -= action.ScaledUtilityScore;
            }
        }

        if (selectedAction == null)
        {
            AgentLogger.LogError("No suitable action found!");
            return null;
        }

        // Compare the selected action with the max utility action
        if (selectedAction != maxUtilityAction)
        {
            AgentLogger.LogWarning(
                $"Roulette selection picked a lower utility action: {selectedAction.name} "
                    + $"(Utility: {selectedAction.ScaledUtilityScore}) vs Max Utility Action: {maxUtilityAction.name} "
                    + $"(Utility: {maxUtilityAction.ScaledUtilityScore})."
            );
        }

        // Check if the selected action has a super low utility score
        if (
            selectedAction.ScaledUtilityScore < total * 0.1f
            || selectedAction.ScaledUtilityScore < maxUtilityAction.ScaledUtilityScore * 0.1f
        )
        {
            AgentLogger.LogWarning(
                $"Super low utility action selected! Action: {selectedAction.name} (Utility: {selectedAction.ScaledUtilityScore}) "
                    + $"vs Max Utility Action: {maxUtilityAction.name} "
                    + $"(Utility: {maxUtilityAction.ScaledUtilityScore})."
            );
        }

        return selectedAction;
    }
}
