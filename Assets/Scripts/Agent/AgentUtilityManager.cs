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
        return;

        foreach (AgentAction action in actions)
        {
            action.utilityScore = 1.0f / actions.Count;
        }
        DebugManager.Instance.Log(transform, "Reset utilScores", Color.red);
    }

    public void CalculateUtilityScores()
    {
        foreach (AgentAction action in actions)
        {
            action.CalculateUtility(_agent, _agent.AgentMetrics);
            action.RestoreUtilityOverTime();
            NormalizeUtilityScores();
        }
    }

    public void AddCooldowns()
    {
        foreach (AgentAction action in actions)
        {
            action.AddCooldown();
        }
    }

    void AdjustUtilityScore(AgentAction action, float amount)
    {
        if (action == null)
            return;

        action.utilityScore = Mathf.Clamp(action.utilityScore + amount * Time.deltaTime, 0, 1);
        NormalizeUtilityScores();
    }

    public void NormalizeUtilityScores()
    {
        float sum = actions.Sum(action => action.utilityScore);
        float minScore = 0.01f;

        // Prevent division by zero
        if (sum == 0)
            return;

        // Scale each score relative to total sum
        // Also ensure no score is below minScore
        foreach (AgentAction action in actions)
        {
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
