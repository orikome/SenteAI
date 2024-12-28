using System;
using UnityEngine;

public class ActionBehaviour : MonoBehaviour
{
    protected Agent _agent;
    public Action OnHitCallback;
    public Action OnMissCallback;
    protected bool hasHitTarget = false;
    protected LayerMask _targetMask;
    protected LayerMask _ownerMask;

#if UNITY_EDITOR
    private void Start()
    {
        if (_targetMask == 0 || _ownerMask == 0)
        {
            if (_agent == null)
                return;
            AgentLogger.LogError("Target or Owner mask is not set");
        }
    }
#endif

    public virtual void Initialize(Agent agent)
    {
        if (agent == null || agent.Target == null)
        {
            AgentLogger.LogWarning("Agent destroyed before projectile got initialized");
            return;
        }

        _agent = agent;
        _targetMask = Helpers.GetTargetMask(_agent.Faction);
        _ownerMask = Helpers.GetOwnerMask(_agent.Faction);
        Helpers.SetLayerRecursively(gameObject, Helpers.GetProjectileLayer(_agent.Faction));
    }
}
