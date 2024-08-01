using System.Linq;
using UnityEngine;

public class RouletteSelectionStrategy : ActionSelectionStrategy
{
    public override AgentAction SelectAction(Agent agent)
    {
        // Roulette wheel picker with sizes proportional to action weights

        // TODO: If a low probability action gets picked
        // and its effectiveness is low -> trigger "oopsie" dialogue
        float total = agent.actionWeightManager.weights.Values.Sum();
        float randomPoint = Random.value * total;

        foreach (var entry in agent.actionWeightManager.weights)
        {
            if (randomPoint < entry.Value)
            {
                return entry.Key;
            }
            else
            {
                randomPoint -= entry.Value;
            }
        }

        Debug.LogError("No suitable action found!");
        return null;
    }
}
