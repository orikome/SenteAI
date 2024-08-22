using UnityEngine;

public abstract class SenseModule : AgentModule
{
    [SerializeField]
    public Vector3 LastKnownLocation { get; protected set; }
    public float LastSeen { get; protected set; }
    public bool CanSenseTarget { get; set; }
}
