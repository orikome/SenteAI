using UnityEngine;

public abstract class AgentAction : ScriptableObject
{
    // Set these in editor
    //[Range(0, 100)]
    //public int cost;
    public float cooldownTime = 0.1f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    protected float _baseUtility; // Keep between 0.01f - 1.0f

    [Range(0.0f, 1.0f)]
    public float decayFactor = 0.9f; // Retain 90% of utility after execution

    [Range(0.0f, 1.0f)]
    public float restoreRate = 0.2f;

    // Handled in code
    public readonly float MIN_UTILITY = 0.01f;
    public readonly float MAX_UTILITY = 1.0f;
    public float LastExecutedTime { get; protected set; }

    [Range(0.0f, 1.0f)]
    public float utilityScore;

    public abstract bool CanExecute(Agent agent);

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

    public abstract void Initialize(Agent agent);

    /// <summary>
    /// Called every frame in the agent's update loop.
    /// </summary>
    public abstract void Execute(Transform firePoint, Agent agent);

    /// <summary>
    /// Called every frame in the agent's update loop.
    /// </summary>
    public virtual void CalculateUtility(Agent agent, AgentMetrics context) { }

    /// <summary>
    /// Applies decay to the utilityScore.
    /// </summary>
    protected void ApplyDecay()
    {
        utilityScore = Mathf.Max(utilityScore * decayFactor, MIN_UTILITY);
    }

    public void RestoreUtilityOverTime()
    {
        if (Time.time - LastExecutedTime > cooldownTime)
        {
            utilityScore = Mathf.Min(utilityScore + restoreRate * Time.deltaTime, _baseUtility);
        }
    }

    public void AfterExecution()
    {
        AddCooldown();
        ApplyDecay();
        RestoreUtilityOverTime();
    }
}
