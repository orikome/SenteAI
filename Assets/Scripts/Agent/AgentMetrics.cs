using System.Collections.Generic;
using UnityEngine;

public class AgentMetrics : MonoBehaviour
{
    public float DistanceToPlayer { get; private set; }
    public float HealthFactor { get; private set; }
    public float EnergyLevel { get; private set; }
    public List<AgentAction> actionHistory = new();
    public Vector3 velocity;
    public Vector3 lastPosition = Vector3.zero;

    void Update()
    {
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }

    public void SetDistanceToPlayer(float disToPlayer)
    {
        DistanceToPlayer = disToPlayer;
    }

    public void AddActionToHistory(AgentAction action)
    {
        actionHistory.Add(action);
        if (actionHistory.Count > 20)
        {
            actionHistory.RemoveAt(0);
        }
    }
}
