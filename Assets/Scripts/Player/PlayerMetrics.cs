using System.Collections.Generic;
using UnityEngine;

public class PlayerMetrics : MonoBehaviour
{
    public enum PlayerBehavior
    {
        Aggressive,
        Defensive,
        Balanced
    }

    [Header("Player Metrics")]
    public float shootingFrequency;
    public float dodgeRatio;
    public float distanceToClosestEnemy;
    public float movementSpeed;
    public Vector3 velocity;
    public float damageTaken;
    public bool isInCover { get; private set; }
    public float timeInCover;
    public Vector3 lastPosition = Vector3.zero;

    [Header("Player Position History")]
    public List<Vector3> positionHistory = new List<Vector3>();
    private float historyRecordInterval = 0.2f;
    private float timeSinceLastRecord = 0f;
    private readonly int maxHistoryCount = 200;

    [Header("Behavior Thresholds")]
    [SerializeField]
    private float aggressiveThreshold = 0.7f;

    [SerializeField]
    private float defensiveThreshold = 0.3f;

    public PlayerBehavior currentBehavior;
    Agent closestEnemy;

    void Start()
    {
        lastPosition = transform.position;
        closestEnemy = null;
    }

    void Update()
    {
        UpdatePlayerMetrics();
        TrackPlayerPositionHistory();
        currentBehavior = ClassifyBehavior();
        RespondToPlayerBehavior();
    }

    public void UpdateCoverStatus(bool canAnyEnemySeePlayer)
    {
        isInCover = !canAnyEnemySeePlayer;
    }

    void UpdatePlayerMetrics()
    {
        shootingFrequency = Random.Range(0f, 1f);
        dodgeRatio = Random.Range(0f, 1f);

        if (isInCover)
            timeInCover += Time.deltaTime;

        FindClosestEnemyToPlayer();

        movementSpeed = Vector3.Distance(transform.position, lastPosition) / Time.deltaTime;
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }

    void TrackPlayerPositionHistory()
    {
        timeSinceLastRecord += Time.deltaTime;

        if (timeSinceLastRecord >= historyRecordInterval)
        {
            positionHistory.Add(transform.position);

            // If over limit, start removing past records
            if (positionHistory.Count > maxHistoryCount)
            {
                positionHistory.RemoveAt(0);
            }

            timeSinceLastRecord = 0f;
        }
    }

    public Transform FindClosestEnemyToPlayer()
    {
        if (GameManager.Instance.activeAgents.Count == 0)
            return Player.Instance.transform;

        float closestEnemyDistance = Mathf.Infinity;

        foreach (Agent agent in GameManager.Instance.activeAgents)
        {
            float distance = OrikomeUtils.GeneralUtils.GetDistanceSquared(
                agent.transform.position,
                transform.position
            );

            if (distance < closestEnemyDistance)
            {
                closestEnemyDistance = distance;
                closestEnemy = agent;
                distanceToClosestEnemy = distance;
            }
        }

        return closestEnemy.transform;
    }

    public Vector3 GetAveragePosition()
    {
        Vector3 averagePosition = Vector3.zero;
        foreach (Vector3 pos in positionHistory)
        {
            averagePosition += pos;
        }
        return averagePosition / positionHistory.Count;
    }

    public Vector3 GetAveragePosition(int historyCount)
    {
        int validHistoryCount = Mathf.Min(historyCount, positionHistory.Count);

        if (validHistoryCount == 0)
            return transform.position;

        Vector3 averagePosition = Vector3.zero;

        for (int i = positionHistory.Count - validHistoryCount; i < positionHistory.Count; i++)
        {
            averagePosition += positionHistory[i];
        }

        return averagePosition / validHistoryCount;
    }

    public Vector3 PredictNextPositionUsingAverage()
    {
        Vector3 avgPosition = GetAveragePosition();
        return (avgPosition + transform.position) / 2;
    }

    public Vector3 PredictNextPositionUsingVelocity()
    {
        if (positionHistory.Count < 2)
            return transform.position;

        Vector3 lastVelocity =
            (
                positionHistory[positionHistory.Count - 1]
                - positionHistory[positionHistory.Count - 2]
            ) / historyRecordInterval;

        // Add velocity to current position
        Vector3 predictedPosition = transform.position + lastVelocity * historyRecordInterval;

        return predictedPosition;
    }

    public Vector3 PredictNextPositionUsingMomentum()
    {
        if (positionHistory.Count < 3)
            return transform.position;

        // Calculate velocity between last two positions
        Vector3 velocity1 =
            (
                positionHistory[positionHistory.Count - 1]
                - positionHistory[positionHistory.Count - 2]
            ) / historyRecordInterval;

        // Calculate velocity one step further back
        Vector3 velocity2 =
            (
                positionHistory[positionHistory.Count - 2]
                - positionHistory[positionHistory.Count - 3]
            ) / historyRecordInterval;

        Vector3 acceleration = (velocity1 - velocity2) / historyRecordInterval;

        // Project both velocity and acceleration forward
        Vector3 predictedPosition =
            transform.position
            + velocity1
            + 0.5f * acceleration * Mathf.Pow(historyRecordInterval, 2);

        return predictedPosition;
    }

    PlayerBehavior ClassifyBehavior()
    {
        if (shootingFrequency > aggressiveThreshold && distanceToClosestEnemy < defensiveThreshold)
            return PlayerBehavior.Aggressive;

        if (dodgeRatio > aggressiveThreshold)
            return PlayerBehavior.Defensive;

        return PlayerBehavior.Balanced;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawCube(GetAveragePosition(), Vector3.one * 4);

        Gizmos.color = Color.blue;
        Gizmos.DrawCube(PredictNextPositionUsingMomentum(), Vector3.one * 2);

        // Visualize player history with small spheres
        if (positionHistory.Count > 0)
        {
            Gizmos.color = Color.red;
            foreach (Vector3 pos in positionHistory)
            {
                Gizmos.DrawSphere(pos, 0.4f);
            }
        }

        // Visualize cover status
        Gizmos.color = isInCover ? Color.green : Color.red;
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

    void RespondToPlayerBehavior()
    {
        if (Time.frameCount % 300 != 0)
            return;

        switch (currentBehavior)
        {
            case PlayerBehavior.Aggressive:
                DebugManager.Instance.Log(transform, "Player is Aggressive", Color.red);
                break;
            case PlayerBehavior.Defensive:
                DebugManager.Instance.Log(transform, "Player is Defensive", Color.blue);
                break;
            case PlayerBehavior.Balanced:
                DebugManager.Instance.Log(transform, "Player is Balanced", Color.green);
                break;
        }
    }
}
