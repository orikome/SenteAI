using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBrain", menuName = "Module/EnemyBrain")]
public class EnemyBrain : Module
{
    private Enemy _enemy;
    private ActionSelectionStrategy _actionSelectionStrategy;
    private float _lastActionTime;
    private readonly float _globalCooldown = 0.4f;

    public override void Initialize(Agent agent)
    {
        _enemy = (Enemy)agent;

        // Initialize the action selection strategy from AgentData
        _actionSelectionStrategy = _enemy.Data.actionSelectionStrategy;
        if (_actionSelectionStrategy == null)
        {
            DebugManager.Instance.LogError("ActionSelectionStrategy is not assigned in AgentData!");
        }

        // Reset utility scores
        ResetUtilityScores();
    }

    public override void Execute(Agent agent)
    {
        // Calculate utility scores for actions
        foreach (var action in _enemy.Actions)
        {
            action.CalculateUtility(_enemy);
        }

        // Normalize utility scores
        NormalizeUtilityScores();

        // Select and execute action
        if (Time.time >= _lastActionTime + _globalCooldown)
        {
            AgentAction decidedAction = _actionSelectionStrategy.SelectAction(_enemy);
            if (decidedAction == null)
            {
                DebugManager.Instance.LogWarning("No valid action selected.");
                return;
            }

            DebugManager.Instance.Log(
                $"Selected: {Helpers.CleanName(decidedAction.name)} with utilScore: {decidedAction.ScaledUtilityScore}"
            );

            if (_enemy.Metrics != null)
            {
                _enemy.Metrics.AddActionToHistory(decidedAction);
            }

            decidedAction.Execute(_enemy.firePoint, Vector3.one);
            _lastActionTime = Time.time;
        }
    }

    private Vector3 GetShootDirection()
    {
        return Vector3.zero;
    }

    private void ResetUtilityScores()
    {
        foreach (AgentAction action in _enemy.Actions)
        {
            action.ScaledUtilityScore = 1.0f / _enemy.Actions.Count;
        }
        DebugManager.Instance.SpawnTextLog(_enemy.transform, "Reset utilScores", Color.red);
    }

    private void NormalizeUtilityScores()
    {
        float sum = _enemy.Actions.Sum(action => action.ScaledUtilityScore);
        float minScore = 0.01f;

        // Prevent division by zero
        if (sum == 0)
            return;

        foreach (AgentAction action in _enemy.Actions)
        {
            // Scale by scaled utility to preserve differences
            action.ScaledUtilityScore = Mathf.Max(action.ScaledUtilityScore / sum, minScore);
        }

        // Ensure scores sum to exactly 1
        sum = _enemy.Actions.Sum(action => action.ScaledUtilityScore);
        foreach (AgentAction action in _enemy.Actions)
        {
            action.ScaledUtilityScore /= sum;
        }
    }
}
