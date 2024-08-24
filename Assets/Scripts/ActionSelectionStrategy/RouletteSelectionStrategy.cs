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
        // Roulette wheel picker with sizes proportional to action utility scores

        // TODO: If a low probability action gets picked
        // and its effectiveness is low -> trigger "oopsie" dialogue
        var executableActions = agent
            .ActionUtilityManager.actions.Where(action => action.CanExecute(agent))
            .ToList();

        if (executableActions.Count == 0)
        {
            Debug.LogError("No executable actions available!");
            return null;
        }

        float total = executableActions.Sum(action => action.utilityScore);
        float randomPoint = Random.value * total;

        foreach (var action in executableActions)
        {
            if (randomPoint < action.utilityScore)
            {
                return action;
            }
            else
            {
                randomPoint -= action.utilityScore;
            }
        }

        Debug.LogError("No suitable action found!");
        return null;
    }
}
