using UnityEngine;

public abstract class PerceptionModule : AgentModule
{
    [SerializeField]
    protected Transform target;
    public Vector3 lastKnownLocation { get; protected set; }
    public float lastSeen;
}
