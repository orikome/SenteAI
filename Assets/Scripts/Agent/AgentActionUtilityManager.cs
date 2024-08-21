using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgentActionUtilityManager : MonoBehaviour
{
    public List<AgentAction> actions;

    public void Initialize()
    {
        InitializeUtilityScore();
    }

    private void InitializeUtilityScore()
    {
        // Equalize utility scores
        foreach (AgentAction action in actions)
        {
            action.utilityScore = 1.0f / actions.Count;
        }
    }

    public void AdjustUtilityScore(AgentAction action, float amount)
    {
        if (action == null)
            return;

        action.utilityScore = Mathf.Clamp(action.utilityScore + amount * Time.deltaTime, 0, 1);
        NormalizeUtilityScore();
    }

    public void NormalizeUtilityScore()
    {
        float sum = actions.Sum(action => action.utilityScore);
        float minScore = 0.01f;

        // Prevent division by zero
        if (sum == 0)
            return;

        // Normalize utility scores
        foreach (AgentAction action in actions)
        {
            action.utilityScore = Mathf.Max(action.utilityScore / sum, minScore);
        }

        // Ensure the scores sum to 1
        sum = actions.Sum(action => action.utilityScore);
        foreach (AgentAction action in actions)
        {
            action.utilityScore /= sum;
        }
    }

    public void ResetUtilityScore()
    {
        InitializeUtilityScore();
        DebugManager.Instance.Log(transform, "Reset utililityScore", Color.red);
    }
}
