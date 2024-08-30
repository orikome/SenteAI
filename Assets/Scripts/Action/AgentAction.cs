using System.Security.Cryptography;
using UnityEngine;

public abstract class AgentAction : ScriptableObject
{
    [Range(0, 100)]
    public int cost;

    [Range(0.0f, 1.0f)]
    public float utilityScore;
    public float _baseUtility; // Keep between 0.01f - 1.0f
    public readonly float MIN_UTILITY = 0.01f;
    public float lastExecutedTime = 0f;
    public float cooldownTime = 0.1f;
    public abstract bool CanExecute(Agent agent);

    /// <summary>
    /// Returns how far along the cooldown is between 0 and 1.
    /// </summary>
    public float GetCooldownProgress()
    {
        return Mathf.Clamp01((Time.time - lastExecutedTime) / cooldownTime);
    }

    /// <summary>
    /// Returns the direct value of how much time is left on a cooldown.
    /// </summary>
    public float GetCooldownTimeRemaining()
    {
        return cooldownTime - (Time.time - lastExecutedTime);
    }

    public virtual bool IsOnCooldown()
    {
        return Time.time - lastExecutedTime < cooldownTime;
    }

    public virtual void AddCooldown()
    {
        cooldownTime = lastExecutedTime;
    }

    public virtual void ResetCooldown()
    {
        cooldownTime = 0;
    }

    public abstract void Initialize(Agent agent);

    /// <summary>
    /// Called every frame in the agent's update loop.
    /// </summary>
    public abstract void ExecuteLoop(Transform firePoint, Agent agent);

    /// <summary>
    /// Called every frame in the agent's update loop.
    /// </summary>
    public virtual float CalculateUtility(Agent agent, AgentMetrics context)
    {
        return -1.0f;
    }
}
