using UnityEngine;

public abstract class AgentAction : ScriptableObject
{
    // -- Set these in editor --
    public AgentState agentState = AgentState.Combat;
    public float cooldownTime = 0.1f;

    [Range(0.0f, 1.0f)]
    public float biasWeight;

    [Range(0.0f, 1.0f)]
    public float penaltyPerExecution = 0.2f;

    [Range(0.0f, 1.0f)]
    public float penaltyRestoreRate = 0.2f;

    // -- Handled in code --
    public const float MIN_UTILITY = 0.01f;
    public const float MAX_UTILITY = 1.0f;
    public float PenaltyFactor { get; private set; } = 0.0f;
    public float UnscaledUtilityScore { get; private set; }
    public float LastExecutedTime { get; protected set; }
    public float ScaledUtilityScore { get; set; }
    public int TimesExecuted { get; private set; } = 0;
    private bool _penaltyMaxedOut = false;
    protected Agent _agent;

    public virtual bool CanExecute(Agent agent)
    {
        // If penalty is maxed out, block execution until PenaltyFactor is fully restored
        if (_penaltyMaxedOut)
            return false;

        // If agent or agent's target is null, return false
        if (_agent == null || _agent.Target == null)
            return false;

        // Return true if not on cooldown and utility is above minimum
        return !IsOnCooldown() && ScaledUtilityScore > MIN_UTILITY;
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
        _agent = agent;
    }

    /// <summary>
    /// Called every frame in the agent's update loop.
    /// </summary>
    public abstract void Execute(Transform firePoint, Vector3 direction);

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
                $"Penalty exceeded, applying a penalty period to: {Helpers.CleanName(name)}.",
                _agent.gameObject
            );
            _penaltyMaxedOut = true; // Prevent further execution until penalty is cleared
        }
    }

    public void RestorePenaltyAndFeedback()
    {
        if (PenaltyFactor > 0.0f)
        {
            PenaltyFactor -= penaltyRestoreRate * Time.deltaTime;
            if (PenaltyFactor <= 0.0f)
            {
                _penaltyMaxedOut = false;
                DebugManager.Instance.Log(
                    $"{Helpers.CleanName(name)} penalty has been fully restored.",
                    _agent.gameObject
                );
            }
        }

        if (this is not IFeedbackAction feedbackAction)
            return;

        // Restore feedback modifier slower
        float feedbackRestoreRate = penaltyRestoreRate / 4.0f;
        float feedbackModifier = feedbackAction.FeedbackModifier;

        // Restore feedback modifier to 1.0f
        if (feedbackModifier > 1.0f)
            feedbackAction.FeedbackModifier = Mathf.Max(
                1.0f,
                feedbackModifier - feedbackRestoreRate * Time.deltaTime
            );
        else if (feedbackModifier < 1.0f)
            feedbackAction.FeedbackModifier = Mathf.Min(
                1.0f,
                feedbackModifier + feedbackRestoreRate * Time.deltaTime
            );
    }

    public virtual void SetUtilityWithModifiers(float util)
    {
        UnscaledUtilityScore = util;

        // 1. Apply cooldown factor
        if (GetCooldownProgress() < 1.0f)
        {
            // If on cooldown, scale by cooldown progress
            util *= GetCooldownProgress();
        }

        // 2. Apply bias
        util *= biasWeight;

        // 3. Apply penalty factor, if any
        util *= Mathf.Max(MAX_UTILITY - PenaltyFactor, MIN_UTILITY);

        // 4. Apply feedback modifier, if action has feedback methods
        if (this is IFeedbackAction feedbackAction)
        {
            util *= feedbackAction.ApplyFeedbackModifier(util, feedbackAction);
        }

        // 5. Restore penalty and feedback modifiers back to default
        RestorePenaltyAndFeedback();

        if (util <= 0)
            DebugManager.Instance.LogWarning(
                $"[Frame {Time.frameCount}] Utility of {Helpers.CleanName(name)} is zero or negative, check parameters."
            );

        // 6. Utility has been scaled, set it
        ScaledUtilityScore = Mathf.Max(util, MIN_UTILITY);
    }

    public void AfterExecution()
    {
        AddCooldown();
        AddPenalty();
        TimesExecuted++;
    }
}
