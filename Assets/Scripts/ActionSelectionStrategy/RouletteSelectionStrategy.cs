using System.Linq;
using UnityEngine;

[CreateAssetMenu(
    fileName = "RouletteSelectionStrategy",
    menuName = "ActionSelection/RouletteSelectionStrategy"
)]
public class RouletteSelectionStrategy : ActionSelectionStrategy
{
    public override AgentAction SelectAction(Agent agent)
    {
        // Roulette wheel picker with sizes proportional to utility scores

        var executableActions = agent.Actions.Where(action => action.CanExecute(agent)).ToList();

        if (executableActions.Count == 0)
        {
            Debug.LogWarning("No executable actions available!");
            return null;
        }

        // Find the action with the highest utility
        var maxUtilityAction = executableActions
            .OrderByDescending(action => action.utilityScore)
            .First();
        float total = executableActions.Sum(action => action.utilityScore);
        float randomPoint = Random.value * total;

        AgentAction selectedAction = null;

        foreach (var action in executableActions)
        {
            if (randomPoint < action.utilityScore)
            {
                selectedAction = action;
                break;
            }
            else
            {
                randomPoint -= action.utilityScore;
            }
        }

        if (selectedAction == null)
        {
            Debug.LogError("No suitable action found!");
            return null;
        }

        // Compare the selected action with the max utility action
        if (selectedAction != maxUtilityAction)
        {
            Debug.LogWarning(
                $"Roulette selection picked a lower utility action: {selectedAction.name} "
                    + $"(Utility: {selectedAction.utilityScore}) vs Max Utility Action: {maxUtilityAction.name} "
                    + $"(Utility: {maxUtilityAction.utilityScore})."
            );
        }

        // Check if the selected action has a super low utility score
        if (
            selectedAction.utilityScore < total * 0.1f
            || selectedAction.utilityScore < maxUtilityAction.utilityScore * 0.1f
        )
        {
            // TODO: If a low probability action gets picked
            // and its effectiveness is low -> trigger "oopsie" dialogue
            Debug.LogError(
                $"Super low utility action selected! Action: {selectedAction.name} (Utility: {selectedAction.utilityScore}) "
                    + $"vs Max Utility Action: {maxUtilityAction.name} "
                    + $"(Utility: {maxUtilityAction.utilityScore})."
            );
        }

        return selectedAction;
    }
}
