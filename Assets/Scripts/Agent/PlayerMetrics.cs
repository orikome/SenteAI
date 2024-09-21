using System.Collections.Generic;
using UnityEngine;

public class PlayerMetrics : Metrics
{
    public bool IsInCover { get; private set; }
    public Vector3 PredictedPosition { get; private set; }
    public float TimeInCover { get; private set; }
    public float ShootingFrequency { get; private set; }
    public float DodgeRatio { get; private set; }
    public float DistanceToClosestEnemy { get; private set; }

    // Player position related
    public List<Vector3> PositionHistory { get; private set; } = new();
    private readonly float historyRecordInterval = 0.2f;
    private float timeSinceLastRecord = 0f;
    private readonly int maxHistoryCount = 200;
    private readonly float detectionThreshold = 1.5f;
    private readonly int recentHistorySize = 6;
    private Agent closestEnemy;

    void Start()
    {
        LastPosition = transform.position;
        closestEnemy = null;

        for (int i = 0; i < recentHistorySize; i++)
        {
            PositionHistory.Add(transform.position);
        }
    }

    void Update()
    {
        ShootingFrequency = Random.Range(0f, 1f);
        DodgeRatio = Random.Range(0f, 1f);

        if (IsInCover)
            TimeInCover += Time.deltaTime;

        FindClosestEnemyToPlayer();
        UpdateVelocity();
        TrackPlayerPositionHistory();
        CurrentBehavior = ClassifyBehavior();
    }

    public void UpdateCoverStatus(bool canAnyEnemySeePlayer)
    {
        IsInCover = !canAnyEnemySeePlayer;
        TimeInCover = 0;
    }

    void TrackPlayerPositionHistory()
    {
        timeSinceLastRecord += Time.deltaTime;

        if (timeSinceLastRecord >= historyRecordInterval)
        {
            PositionHistory.Add(transform.position);

            // If over limit, start removing past records
            if (PositionHistory.Count > maxHistoryCount)
            {
                PositionHistory.RemoveAt(0);
            }

            timeSinceLastRecord = 0f;
        }
    }

    public Transform FindClosestEnemyToPlayer()
    {
        if (GameManager.Instance.activeEnemies.Count == 0)
            return null;

        float closestEnemyDistance = Mathf.Infinity;

        foreach (Agent enemy in GameManager.Instance.activeEnemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, transform.position);

            EnemyMetrics enemyMetrics = (EnemyMetrics)enemy.Metrics;
            enemyMetrics.SetDistanceToPlayer(distance);

            if (distance < closestEnemyDistance)
            {
                closestEnemyDistance = distance;
                closestEnemy = enemy;
                DistanceToClosestEnemy = distance;
            }
        }

        return closestEnemy.transform;
    }

    public Vector3 GetAveragePosition()
    {
        Vector3 averagePosition = Vector3.zero;
        foreach (Vector3 pos in PositionHistory)
        {
            averagePosition += pos;
        }
        return averagePosition / PositionHistory.Count;
    }

    public Vector3 GetAveragePosition(int historyCount)
    {
        int validHistoryCount = Mathf.Min(historyCount, PositionHistory.Count);

        if (validHistoryCount == 0)
            return transform.position;

        Vector3 averagePosition = Vector3.zero;

        for (int i = PositionHistory.Count - validHistoryCount; i < PositionHistory.Count; i++)
        {
            averagePosition += PositionHistory[i];
        }

        return averagePosition / validHistoryCount;
    }

    public Vector3 PredictNextPositionUsingMomentum()
    {
        // Calculate velocity between last two positions
        Vector3 velocity1 =
            (
                PositionHistory[PositionHistory.Count - 1]
                - PositionHistory[PositionHistory.Count - 2]
            ) / historyRecordInterval;

        // Calculate velocity one step further back
        Vector3 velocity2 =
            (
                PositionHistory[PositionHistory.Count - 2]
                - PositionHistory[PositionHistory.Count - 3]
            ) / historyRecordInterval;

        Vector3 acceleration = (velocity1 - velocity2) / historyRecordInterval;

        // Project both velocity and acceleration forward
        Vector3 predictedPosition =
            transform.position
            + velocity1
            + 0.5f * Mathf.Pow(historyRecordInterval, 2) * acceleration;

        PredictedPosition = predictedPosition;
        return predictedPosition;
    }

    public Vector3 PredictPositionDynamically()
    {
        // If player is cheesing (circling or moving in a small area), use average position prediction
        if (IsClusteredMovement())
            return GetAveragePosition(recentHistorySize);

        return PredictNextPositionUsingMomentum();
    }

    private bool IsClusteredMovement()
    {
        // Get average position of last few locations
        Vector3 averagePosition = GetAveragePosition(recentHistorySize);
        float totalDisplacement = 0f;

        for (int i = PositionHistory.Count - recentHistorySize; i < PositionHistory.Count; i++)
        {
            totalDisplacement += Vector3.Distance(PositionHistory[i], averagePosition);
        }

        float averageDisplacement = totalDisplacement / recentHistorySize;

        // If below threshold, player positions are clustered
        return averageDisplacement < detectionThreshold;
    }

    protected override Behavior ClassifyBehavior()
    {
        if (Time.frameCount % 500 != 0)
            return Behavior.Balanced;

        if (ShootingFrequency > AggressiveThreshold && DistanceToClosestEnemy < DefensiveThreshold)
        {
            DebugManager.Instance.SpawnTextLog(transform, "Player is Aggressive", Color.red);
            return Behavior.Aggressive;
        }

        if (DodgeRatio > AggressiveThreshold)
        {
            DebugManager.Instance.SpawnTextLog(transform, "Player is Defensive", Color.blue);
            return Behavior.Defensive;
        }

        DebugManager.Instance.SpawnTextLog(transform, "Player is Balanced", Color.green);
        return Behavior.Balanced;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.DrawCube(GetAveragePosition(), Vector3.one * 4);

        if (IsClusteredMovement())
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(GetAveragePosition(recentHistorySize), Vector3.one * 4);
        }
        else
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(PredictNextPositionUsingMomentum(), Vector3.one * 2);
        }

        // Visualize player history with small spheres
        if (PositionHistory.Count > 0)
        {
            Gizmos.color = Color.red;
            foreach (Vector3 pos in PositionHistory)
            {
                Gizmos.DrawSphere(pos, 0.4f);
            }
        }

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
