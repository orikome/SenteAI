using System.Collections.Generic;
using UnityEngine;

public class AllyMetrics : Metrics
{
    public float DistanceToPlayer { get; private set; }
    public Transform NearestEnemy { get; private set; }
    public float HealthFactor { get; private set; }
    public float EnergyLevel { get; private set; }
    public List<AgentAction> ActionHistory { get; private set; } = new();

    void Update()
    {
        CurrentBehavior = ClassifyBehavior();
        UpdateVelocity();
        UpdateNearestEnemy();
        SetDistanceToTarget();
    }

    public void SetDistanceToTarget()
    {
        // TEMPORARY!!!
        DistanceToTarget = Vector3.Distance(
            transform.position,
            GetComponent<Agent>().Target.transform.position
        );
    }

    private void UpdateNearestEnemy()
    {
        float nearestDistance = float.MaxValue;
        Transform nearestEnemy = null;

        foreach (var enemy in GameManager.Instance.activeEnemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }

        NearestEnemy = nearestEnemy;
    }

    public void AddActionToHistory(AgentAction action)
    {
        ActionHistory.Add(action);
        if (ActionHistory.Count > 20)
        {
            ActionHistory.RemoveAt(0);
        }
    }
}
