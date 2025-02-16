using System.Linq;
using UnityEngine;

namespace SenteAI.Core
{
    [CreateAssetMenu(
        fileName = "RandomSelectionStrategy",
        menuName = "SenteAI/SelectionStrategies/RandomSelectionStrategy"
    )]
    public class RandomSelectionStrategy : ActionSelectionStrategy
    {
        public override AgentAction SelectAction(Agent agent)
        {
            var executableActions = agent.Actions.Where(action => action.CanExecute()).ToList();

            if (executableActions.Count == 0)
            {
                AgentLogger.LogWarning("No executable actions available!");
                return null;
            }

            int randomIndex = Random.Range(0, executableActions.Count);
            return executableActions[randomIndex];
        }
    }
}
