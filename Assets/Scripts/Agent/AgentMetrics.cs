using System.Collections.Generic;
using UnityEngine;

public class AgentMetrics : MonoBehaviour
{
    public float DistanceToPlayer { get; private set; }
    public float HealthFactor { get; private set; }
    public float EnergyLevel { get; private set; }
    public List<AgentAction> actionHistory = new();

    public void SetDistanceToPlayer(float disToPlayer)
    {
        DistanceToPlayer = disToPlayer;
    }
}
