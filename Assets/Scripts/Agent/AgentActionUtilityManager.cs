using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgentActionUtilityManager : MonoBehaviour
{
    public List<AgentAction> actions;
    public Dictionary<AgentAction, float> utilityScore = new Dictionary<AgentAction, float>();

    public void Initialize()
    {
        InitializeUtilityScore();
    }

    private void InitializeUtilityScore()
    {
        // Equalize utility scores
        foreach (AgentAction action in actions)
        {
            utilityScore[action] = 1.0f / actions.Count;
        }
    }

    public void AdjustUtilityScore(AgentAction action, float amount)
    {
        if (utilityScore.ContainsKey(action))
        {
            utilityScore[action] = Mathf.Clamp(
                utilityScore[action] + amount * Time.deltaTime,
                0,
                1
            );
            NormalizeUtilityScore();
        }
    }

    public void NormalizeUtilityScore()
    {
        float sum = utilityScore.Values.Sum();
        float minWeight = 0.01f;

        if (sum > 0) // Prevent division by zero
        {
            foreach (var key in utilityScore.Keys.ToList())
            {
                utilityScore[key] = Mathf.Max(utilityScore[key] / sum, minWeight);
            }

            sum = utilityScore.Values.Sum();
            foreach (var key in utilityScore.Keys.ToList())
            {
                utilityScore[key] = utilityScore[key] / sum;
            }
        }
    }

    public void ResetUtilityScore()
    {
        InitializeUtilityScore();
        DebugManager.Instance.Log(transform, "Reset utililityScore", Color.red);
    }
}
