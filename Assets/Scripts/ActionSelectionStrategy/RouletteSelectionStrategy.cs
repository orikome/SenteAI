using System.Linq;
using UnityEngine;

[CreateAssetMenu(
    fileName = "RouletteSelectionStrategy",
    menuName = "ActionSelection/RouletteSelectionStrategy"
)]
public class RouletteSelectionStrategy : ActionSelectionStrategy
{
    public override AgentAction SelectAction(EnemyAgent agent)
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
            Debug.LogError("No suitable action found!");
            return null;
        }

        // Compare the selected action with the max utility action
        if (selectedAction != maxUtilityAction)
        {
            Debug.LogWarning(
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
            DebugManager.Instance.SpawnTextLog(
                agent.transform,
                "LOW utility action chosen!",
                Color.red
            );
            Debug.LogWarning(
                $"Super low utility action selected! Action: {selectedAction.name} (Utility: {selectedAction.ScaledUtilityScore}) "
                    + $"vs Max Utility Action: {maxUtilityAction.name} "
                    + $"(Utility: {maxUtilityAction.ScaledUtilityScore})."
            );
        }

        return selectedAction;
    }
}
