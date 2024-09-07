using UnityEngine;

public abstract class AgentAction : ScriptableObject
{
    // Set these in editor
    //[Range(0, 100)]
    //public int cost;
    public float cooldownTime = 0.1f;

    // Keep between 0.01f - 1.0f
    [Range(0.0f, 1.0f)]
    public float baseUtility;

    [Range(0.0f, 1.0f)]
    public float decayPerExecution = 0.2f;
    public float DecayFactor { get; private set; } = 0.0f;

    [Range(0.0f, 1.0f)]
    public float restoreRate = 0.2f;

    // Handled in code
    public readonly float MIN_UTILITY = 0.01f;
    public readonly float MAX_UTILITY = 1.0f;
    public float LastExecutedTime { get; protected set; }

    [Range(0.0f, 1.0f)]
    public float utilityScore;

    public virtual bool CanExecute(Agent agent)
    {
        return !IsOnCooldown() && utilityScore > MIN_UTILITY;
    }

    /// <summary>
    /// Returns how far along the cooldown is between 0 and 1.
    /// </summary>
    public float GetCooldownProgress()
    {
        return Mathf.Clamp01((Time.time - LastExecutedTime) / cooldownTime);
    }

    /// <summary>
    /// Returns the direct value of how much time is left on a cooldown.
    /// </summary>
    public float GetCooldownTimeRemaining()
    {
        return cooldownTime - (Time.time - LastExecutedTime);
    }

    public virtual bool IsOnCooldown()
    {
        return Time.time - LastExecutedTime < cooldownTime;
    }

    public virtual void AddCooldown()
    {
        LastExecutedTime = Time.time;
    }

    public virtual void ResetCooldown()
    {
        LastExecutedTime = Time.time - cooldownTime;
    }

    public virtual void Initialize(Agent agent)
    {
        ResetCooldown();
    }

    /// <summary>
    /// Called every frame in the agent's update loop.
    /// </summary>
    public abstract void Execute(Transform firePoint, Agent agent);

    /// <summary>
    /// Called every frame in the agent's update loop.
    /// </summary>
    public virtual void CalculateUtility(Agent agent, AgentMetrics context) { }

    /// <summary>
    /// Applies decay to the decay factor.
    /// </summary>
    public void AddDecay()
    {
        if (DecayFactor < MAX_UTILITY)
            DecayFactor += decayPerExecution;
    }

    public void RestoreUtilityOverTime()
    {
        if (DecayFactor > 0.0f)
            DecayFactor -= restoreRate * Time.deltaTime;
    }

    public void AfterExecution()
    {
        Debug.Log($"[Frame {Time.frameCount}] Applied cooldown and decay to: {name}.");
        AddCooldown();
        AddDecay();
    }
}
