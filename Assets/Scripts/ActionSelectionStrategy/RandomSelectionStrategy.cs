using System.Linq;
using UnityEngine;

[CreateAssetMenu(
    fileName = "RandomSelectionStrategy",
    menuName = "ActionSelection/RandomSelectionStrategy"
)]
public class RandomSelectionStrategy : ActionSelectionStrategy
{
    public override AgentAction SelectAction(Agent agent)
    {
        var executableActions = agent
            .ActionUtilityManager.actions.Where(action => action.CanExecute(agent))
            .ToList();

        if (executableActions.Count == 0)
        {
            Debug.LogError("No executable actions available!");
            return null;
        }

        int randomIndex = Random.Range(0, executableActions.Count);
        return executableActions[randomIndex];
    }
}
