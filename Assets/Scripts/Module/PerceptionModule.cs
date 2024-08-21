using UnityEngine;

public abstract class PerceptionModule : AgentModule
{
    [SerializeField]
    public Vector3 lastKnownLocation { get; protected set; }
    public float lastSeen;
}
