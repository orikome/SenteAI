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
            .Actions.Where(action => action.CanExecute((Enemy)agent))
            .ToList();

        if (executableActions.Count == 0)
        {
            Debug.LogWarning("No executable actions available!");
            return null;
        }

        int randomIndex = Random.Range(0, executableActions.Count);
        return executableActions[randomIndex];
    }
}
