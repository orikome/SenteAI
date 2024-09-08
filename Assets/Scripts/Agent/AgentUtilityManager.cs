using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgentUtilityManager : MonoBehaviour
{
    public List<AgentAction> actions;
    Agent _agent;

    public void AddAction(AgentAction action)
    {
        actions.Add(action);
    }

    public void RemoveAction(AgentAction action)
    {
        actions.Remove(action);
    }

    public void ClearActions()
    {
        actions.Clear();
    }

    public void Initialize()
    {
        _agent = GetComponent<Agent>();
        ResetUtilityScores();
    }

    public void ResetUtilityScores()
    {
        foreach (AgentAction action in actions)
        {
            action.utilityScore = 1.0f / actions.Count;
        }
        DebugManager.Instance.SpawnTextLog(transform, "Reset utilScores", Color.red);
    }

    public void CalculateUtilityScores()
    {
        foreach (AgentAction action in actions)
        {
            action.CalculateUtility(_agent);
        }
        //NormalizeUtilityScores();
    }

    public void NormalizeUtilityScores()
    {
        float sum = actions.Sum(action => action.utilityScore);
        //Debug.Log($"Total util sum before normalization: {sum}");
        float minScore = 0.01f;

        // Prevent division by zero
        if (sum == 0)
            return;

        foreach (AgentAction action in actions)
        {
            // Scale by base utility to preserve differences
            action.utilityScore = Mathf.Max(action.utilityScore / sum, minScore);
        }

        // Ensure scores sum to exactly 1
        sum = actions.Sum(action => action.utilityScore);
        foreach (AgentAction action in actions)
        {
            action.utilityScore /= sum;
        }
    }
}
