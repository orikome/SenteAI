using UnityEngine;

public abstract class SenseModule : Module
{
    [SerializeField]
    public Vector3 LastKnownPosition { get; protected set; }
    public Vector3 LastKnownVelocity { get; protected set; }
    public float LastSeenTime { get; protected set; }
    public bool CanSenseTarget { get; set; }
}
