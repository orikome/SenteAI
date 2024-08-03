using UnityEngine;

public abstract class PerceptionModule : AgentModule
{
    [SerializeField]
    protected Transform target;
    public Vector3 lastKnownLocation { get; protected set; }

    protected virtual void Awake()
    {
        if (target == null)
        {
            target = Player.Instance.transform;
        }
    }
}
