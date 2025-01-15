using System.Collections.Generic;
using UnityEngine;

public class Metrics : MonoBehaviour
{
    public Vector3 Velocity { get; protected set; }
    public Vector3 LastPosition { get; protected set; } = Vector3.zero;
    public float DamageTaken { get; private set; }
    public float DamageDone { get; private set; }
    public float DistanceToTarget { get; protected set; }

    // Behavior parameters
    public Behavior CurrentBehavior { get; protected set; }
    public float AggressiveThreshold { get; protected set; } = 0.7f;
    public float DefensiveThreshold { get; protected set; } = 0.3f;

    //public bool IsInCover { get; private set; }
    //public float TimeInCover { get; private set; }
    public float ShootingFrequency { get; private set; }
    public float DodgeRatio { get; private set; } = 0f;
    public List<Vector3> PositionHistory { get; private set; } = new();
    public List<AgentAction> ActionHistory { get; private set; } = new();

    // Private fields
    private const int RECENT_HISTORY_SIZE = 6;
    private const int MAX_POSITION_HISTORY_COUNT = 50; // 0.2 * 50 = last 10 seconds
    private const int MAX_ACTION_HISTORY_COUNT = 20;
    private const float HISTORY_RECORD_INTERVAL = 0.2f;
    private const float DETECTION_THRESHOLD = 2.5f;
    private Agent _agent;
    private float _timeSinceLastRecord = 0f;

    public void Initialize(Agent agent)
    {
        _agent = agent;
        LastPosition = transform.position;
        for (int i = 0; i < RECENT_HISTORY_SIZE; i++)
        {
            PositionHistory.Add(transform.position);
        }
    }

    public virtual void Update()
    {
        //ShootingFrequency = Random.Range(0f, 1f);
        UpdateDodgeRatio();
        CurrentBehavior = ClassifyBehavior();
        //UpdateVelocity();
        AddPositionToHistory();
        UpdateDistanceToTarget();
    }

    public Vector3 GetDirectionToTarget()
    {
        if (_agent.Target == null)
            return Vector3.zero;

        return _agent.Target.transform.position - _agent.transform.position;
    }

    public Vector3 GetDirectionToTargetPredictedPosition()
    {
        if (_agent.Target == null)
            return Vector3.zero;

        return _agent.Target.Metrics.GetPredictedPosition() - _agent.transform.position;
    }

    public void UpdateDodgeRatio()
    {
        float timeSinceLastDamage = _agent.GetModule<HealthModule>().TimeSinceLastDamage;
        float timeToReachMaxDodge = 10f;
        float regenRate = 0.1f;
        float dodgeTimeRatio = Mathf.Clamp01(timeSinceLastDamage / timeToReachMaxDodge);
        DodgeRatio = Mathf.Lerp(dodgeTimeRatio, 1f, regenRate * Time.deltaTime);
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
        if (Time.frameCount % 500 != 0)
            return Behavior.Balanced;

        if (ShootingFrequency > AggressiveThreshold && DistanceToTarget < DefensiveThreshold)
        {
            //AgentLogger.Log("Player is Aggressive", _agent.gameObject);
            return Behavior.Aggressive;
        }

        if (DodgeRatio > AggressiveThreshold)
        {
            //AgentLogger.Log("Player is Defensive", _agent.gameObject);
            return Behavior.Defensive;
        }

        //AgentLogger.Log("Player is Balanced", _agent.gameObject);
        return Behavior.Balanced;
    }

    protected void AddPositionToHistory()
    {
        _timeSinceLastRecord += Time.deltaTime;

        if (_timeSinceLastRecord >= HISTORY_RECORD_INTERVAL)
        {
            PositionHistory.Add(transform.position);

            // If over limit, start removing past records
            if (PositionHistory.Count > MAX_POSITION_HISTORY_COUNT)
            {
                PositionHistory.RemoveAt(0);
            }

            _timeSinceLastRecord = 0f;
        }
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
            ) / HISTORY_RECORD_INTERVAL;

        // Calculate velocity one step further back
        Vector3 velocity2 =
            (
                PositionHistory[PositionHistory.Count - 2]
                - PositionHistory[PositionHistory.Count - 3]
            ) / HISTORY_RECORD_INTERVAL;

        Vector3 acceleration = (velocity1 - velocity2) / HISTORY_RECORD_INTERVAL;

        // Project both velocity and acceleration forward
        Vector3 predictedPosition =
            transform.position
            + velocity1
            + 0.5f * Mathf.Pow(HISTORY_RECORD_INTERVAL, 2) * acceleration;

        return predictedPosition;
    }

    public Vector3 GetPredictedPosition()
    {
        if (_agent == null)
            return Vector3.zero;

        // If distance is less than 30, directly shoot at player instead of predicting position
        if (DistanceToTarget < 30f)
            return _agent.transform.position;

        // If player is "cheesing" (circling or moving in a small area), use average position prediction
        if (IsClusteredMovement())
            return GetAveragePosition(RECENT_HISTORY_SIZE);

        // Otherwise we predict next position using last few positions
        return PredictNextPositionUsingMomentum();
    }

    public bool IsClusteredMovement()
    {
        // Check if we have enough history first
        if (PositionHistory.Count < RECENT_HISTORY_SIZE)
            return false;

        // Get average position of last few locations
        Vector3 averagePosition = GetAveragePosition(RECENT_HISTORY_SIZE);
        float totalDisplacement = 0f;

        for (int i = PositionHistory.Count - RECENT_HISTORY_SIZE; i < PositionHistory.Count; i++)
        {
            totalDisplacement += Vector3.Distance(PositionHistory[i], averagePosition);
        }

        float averageDisplacement = totalDisplacement / RECENT_HISTORY_SIZE;

        // If below threshold, player positions are clustered
        return averageDisplacement < DETECTION_THRESHOLD;
    }

    public void AddActionToHistory(AgentAction action)
    {
        ActionHistory.Add(action);
        if (ActionHistory.Count > MAX_ACTION_HISTORY_COUNT)
        {
            ActionHistory.RemoveAt(0);
        }
    }

    public void UpdateDistanceToTarget()
    {
        if (_agent == null || _agent.Target == null)
        {
            DistanceToTarget = Mathf.Infinity;
            return;
        }

        DistanceToTarget = Vector3.Distance(transform.position, _agent.Target.transform.position);
    }
}
