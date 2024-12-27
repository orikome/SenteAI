using System.Collections.Generic;
using UnityEngine;

public class PlayerMetrics : Metrics
{
    public bool IsInCover { get; private set; }
    public float TimeInCover { get; private set; }
    public float ShootingFrequency { get; private set; }
    public float DodgeRatio { get; private set; }
    private Agent closestEnemy;

    void Start()
    {
        closestEnemy = null;
    }

    public override void Update()
    {
        base.Update();
        ShootingFrequency = Random.Range(0f, 1f);
        DodgeRatio = Random.Range(0f, 1f);

        if (IsInCover)
            TimeInCover += Time.deltaTime;

        FindClosestEnemyToPlayer();
    }

    public void UpdateCoverStatus(bool canAnyEnemySeePlayer)
    {
        IsInCover = !canAnyEnemySeePlayer;
        TimeInCover = 0;
    }

    public Transform FindClosestEnemyToPlayer()
    {
        if (AgentManager.Instance.activeEnemies.Count == 0)
            return null;

        float closestEnemyDistance = Mathf.Infinity;

        foreach (Agent enemy in AgentManager.Instance.activeEnemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, transform.position);

            if (distance < closestEnemyDistance)
            {
                closestEnemyDistance = distance;
                closestEnemy = enemy;
                DistanceToTarget = distance;
            }
        }

        return closestEnemy.transform;
    }

    protected override Behavior ClassifyBehavior()
    {
        if (Time.frameCount % 500 != 0)
            return Behavior.Balanced;

        if (ShootingFrequency > AggressiveThreshold && DistanceToTarget < DefensiveThreshold)
        {
            AgentLogger.Log("Player is Aggressive", _agent.gameObject);
            return Behavior.Aggressive;
        }

        if (DodgeRatio > AggressiveThreshold)
        {
            AgentLogger.Log("Player is Defensive", _agent.gameObject);
            return Behavior.Defensive;
        }

        AgentLogger.Log("Player is Balanced", _agent.gameObject);
        return Behavior.Balanced;
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // Visualize cover status
        Gizmos.color = IsInCover ? Color.green : Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));

        // Visualize distance to closest enemy
        if (closestEnemy != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, closestEnemy.transform.position);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(closestEnemy.transform.position, 0.5f);
        }
    }
}
