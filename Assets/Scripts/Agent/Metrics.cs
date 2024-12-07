using System.Collections.Generic;
using UnityEngine;

public class Metrics : MonoBehaviour
{
    public Vector3 Velocity { get; protected set; }
    public Vector3 LastPosition { get; protected set; } = Vector3.zero;
    public float DamageTaken { get; private set; }
    public float DamageDone { get; private set; }
    public Vector3 PredictedPosition { get; private set; }
    public float DistanceToTarget { get; protected set; }

    // Behavior parameters
    public Behavior CurrentBehavior { get; protected set; }
    public float AggressiveThreshold { get; protected set; } = 0.7f;
    public float DefensiveThreshold { get; protected set; } = 0.3f;
    public List<Vector3> PositionHistory { get; private set; } = new();
    public List<AgentAction> ActionHistory { get; private set; } = new();
    protected readonly int recentHistorySize = 6;
    protected readonly float historyRecordInterval = 0.2f;
    private readonly float detectionThreshold = 1.5f;
    protected Agent _agent;
    protected float timeSinceLastRecord = 0f;
    protected readonly int maxHistoryCount = 200;

    public void Initialize()
    {
        _agent = GetComponent<Agent>();

        LastPosition = transform.position;
        for (int i = 0; i < recentHistorySize; i++)
        {
            PositionHistory.Add(transform.position);
        }
    }

    public virtual void Update()
    {
        CurrentBehavior = ClassifyBehavior();
        UpdateVelocity();
        RecordAgentPositionHistory();
        SetDistanceToTarget();
    }

    public virtual void UpdateVelocity()
    {
        Velocity = (transform.position - LastPosition) / Time.deltaTime;
        LastPosition = transform.position;
    }

    public void UpdateDamageDone(float dmgDone)
    {
        DamageDone += dmgDone;
    }

    public void UpdateDamageTaken(float dmgTaken)
    {
        DamageTaken += dmgTaken;
    }

    protected virtual Behavior ClassifyBehavior()
    {
        return Behavior.Balanced;
    }

    protected void RecordAgentPositionHistory()
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

    public Vector3 GetAveragePosition()
    {
        if (PositionHistory.Count == 0)
            return transform.position;

        Vector3 averagePosition = Vector3.zero;
        foreach (Vector3 pos in PositionHistory)
        {
            averagePosition += pos;
        }
        return averagePosition / PositionHistory.Count;
    }

    public Vector3 GetAveragePosition(int historyCount)
    {
        if (PositionHistory.Count == 0)
            return transform.position;

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
        // Need at least 3 positions in history for momentum calculation
        if (PositionHistory.Count < 3)
            return transform.position;

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

    public Vector3 PredictPosition()
    {
        // If distance is less than 30, directly shoot at player instead of predicting position
        if (DistanceToTarget < 30f)
            return _agent.transform.position;

        // If player is cheesing (circling or moving in a small area), use average position prediction
        if (IsClusteredMovement())
            return GetAveragePosition(recentHistorySize);

        // Otherwise we predict next position using last few positions
        return PredictNextPositionUsingMomentum();
    }

    protected bool IsClusteredMovement()
    {
        // Check if we have enough history first
        if (PositionHistory.Count < recentHistorySize)
            return false;

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

    public void AddActionToHistory(AgentAction action)
    {
        ActionHistory.Add(action);
        if (ActionHistory.Count > 20)
        {
            ActionHistory.RemoveAt(0);
        }
    }

    public void SetDistanceToTarget()
    {
        DistanceToTarget = Vector3.Distance(transform.position, _agent.Target.transform.position);
    }

    public virtual void OnDrawGizmos()
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
    }
}
