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
    public float unscaledUtility;

    [Range(0.0f, 1.0f)]
    public float penaltyPerExecution = 0.2f;
    public float PenaltyFactor { get; private set; } = 0.0f;

    [Range(0.0f, 1.0f)]
    public float penaltyRestoreRate = 0.2f;

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
    /// Returns how far along the cooldown is between 0f and 1f.
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
    public virtual void CalculateUtility(Agent agent) { }

    /// <summary>
    /// Apply a penalty, a value between 0f and 1f.
    /// </summary>
    public void AddPenalty()
    {
        PenaltyFactor = Mathf.Min(PenaltyFactor + penaltyPerExecution, MAX_UTILITY);

        // If penalty limit is reached, apply a quadruple cooldown
        if (PenaltyFactor >= MAX_UTILITY)
        {
            DebugManager.Instance.Log(
                $"Penalty exceeded, applying QUADRUPLE cooldown to: {Helpers.CleanName(name)}."
            );
            LastExecutedTime = Time.time - cooldownTime + (cooldownTime * 4);
        }
    }

    public void RestorePenaltyOverTime()
    {
        if (PenaltyFactor > 0.0f)
            PenaltyFactor -= penaltyRestoreRate * Time.deltaTime;
    }

    public virtual void SetCalculatedUtility(float util)
    {
        unscaledUtility = util;

        // 1. Apply cooldown factor
        if (GetCooldownProgress() < 1.0f)
        {
            // If on cooldown, scale by cooldown progress
            util *= GetCooldownProgress();
        }

        // 2. Apply base utility
        util *= baseUtility;

        // 3. Apply penalty factor, if any
        util *= Mathf.Max(MAX_UTILITY - PenaltyFactor, MIN_UTILITY);

        // 4. Restore penalty back to normal
        RestorePenaltyOverTime();

        if (util <= 0)
            DebugManager.Instance.LogWarning(
                $"[Frame {Time.frameCount}] Utility of {Helpers.CleanName(name)} is zero or negative, check parameters."
            );

        // 5. Set utility
        utilityScore = Mathf.Max(util, MIN_UTILITY);
    }

    public void AfterExecution()
    {
        DebugManager.Instance.Log($"Cooldown and penalty applied to: {Helpers.CleanName(name)}.");
        AddCooldown();
        AddPenalty();
    }
}
