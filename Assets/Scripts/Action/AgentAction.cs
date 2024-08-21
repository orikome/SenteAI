using UnityEngine;

public abstract class AgentAction : ScriptableObject
{
    [Range(0, 100)]
    public int cost;

    [Range(0.0f, 1.0f)]
    public float utilityScore;
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

    public abstract void Initialize(Agent agent);

    /// <summary>
    /// Called every frame in the agent's update loop.
    /// </summary>
    public abstract void ExecuteActionLoop(Transform firePoint, Agent agent);

    /// <summary>
    /// Called every frame in the agent's update loop.
    /// </summary>
    public virtual void UpdateUtilityLoop(Agent agent) { }
}
