using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgentActionWeightManager : MonoBehaviour
{
    public List<AgentAction> actions;
    public Dictionary<AgentAction, float> weights = new Dictionary<AgentAction, float>();

    public void Initialize()
    {
        InitializeWeights();
    }

    private void InitializeWeights()
    {
        // Equalize weights
        foreach (AgentAction action in actions)
        {
            weights[action] = 1.0f / actions.Count;
        }
    }

    public void AdjustWeight(AgentAction action, float amount)
    {
        if (weights.ContainsKey(action))
        {
            weights[action] = Mathf.Clamp(weights[action] + amount * Time.deltaTime, 0, 1);
            NormalizeWeights();
        }
    }

    public void NormalizeWeights()
    {
        float sum = weights.Values.Sum();
        float minWeight = 0.01f;

        if (sum > 0) // Prevent division by zero
        {
            foreach (var key in weights.Keys.ToList())
            {
                weights[key] = Mathf.Max(weights[key] / sum, minWeight);
            }

            sum = weights.Values.Sum();
            foreach (var key in weights.Keys.ToList())
            {
                weights[key] = weights[key] / sum;
            }
        }
    }

    public void ResetWeights()
    {
        InitializeWeights();
        Debug.Log("Reset weights");
    }
}
