using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SenteAI/Actions/NPCHomingOrbsAction")]
public class NPCHomingOrbsAction : HomingOrbsAction, IFeedbackAction
{
    // Feedback interface
    public Action OnSuccessCallback { get; set; }
    public Action OnFailureCallback { get; set; }
    public int SuccessCount { get; set; } = 0;
    public int FailureCount { get; set; } = 0;
    public float SuccessRate { get; set; } = 1.0f;
    public float FeedbackModifier { get; set; } = 1.0f;

    public override void Execute(Transform firePoint, Vector3 direction)
    {
        ShootOrbs(firePoint);
        AfterExecution();
    }

    public override void CalculateUtility(Agent agent)
    {
        Metrics metrics = agent.Metrics;
        float distance = metrics.DistanceToTarget;
        float maxDistance = 100f;
        float CanSenseFactor = _agent.GetModule<SenseModule>().CanSenseTarget ? 0.6f : 1f;
        float distanceFactor = 1.0f - (distance / maxDistance);
        float calculatedUtil = distanceFactor * 0.5f * CanSenseFactor;
        float calculatedUtilClamped = Mathf.Clamp(calculatedUtil, MIN_UTILITY, MAX_UTILITY);

        SetUtilityWithModifiers(calculatedUtilClamped);
    }

    private void ShootOrbs(Transform firePoint)
    {
        float distanceBetweenOrbs = 3.0f;
        Vector3 rightOffset = firePoint.right * distanceBetweenOrbs;

        for (int i = 0; i < numberOfOrbs; i++)
        {
            float offset = (i - (numberOfOrbs - 1) / 2.0f) * distanceBetweenOrbs;
            Vector3 spawnPosition = firePoint.position + rightOffset * offset;

            float angle = (i - numberOfOrbs / 2) * spreadAngle / numberOfOrbs;
            Quaternion rotation = Quaternion.Euler(0, angle, 0) * firePoint.rotation;
            GameObject orb = Instantiate(orbPrefab, spawnPosition, rotation);

            HomingOrbBehaviour orbComponent = orb.GetComponent<HomingOrbBehaviour>();

            if (orb != null)
            {
                orbComponent.SetParameters(_agent);
                orbComponent.OnHitCallback = () => HandleSuccess(_agent);
                orbComponent.OnMissCallback = () => HandleFailure(_agent);
            }
        }
    }

    public float ApplyFeedbackModifier(float utility, IFeedbackAction feedbackAction)
    {
        if (SuccessRate >= 0.5f)
            // Success rate is good, boost utility
            FeedbackModifier = Mathf.Lerp(1.0f, 1.5f, SuccessRate);
        else
            // Success rate is low, add penalty
            FeedbackModifier = Mathf.Lerp(0.5f, 1.0f, SuccessRate);

        return Mathf.Max(FeedbackModifier, MIN_UTILITY);
    }

    public void HandleFailure(Agent agent)
    {
        // Decrease utility if projectile misses
        FailureCount++;
        OnFailureCallback?.Invoke();
        UpdateSuccessRate();
        int totalAttempts = SuccessCount + FailureCount;
        AgentLogger.Log(
            $"Action {Helpers.CleanName(name)} has failed. Attempts: {totalAttempts}. Success rate: {SuccessRate}, Feedback modifier: {FeedbackModifier}."
        );
    }

    public void HandleSuccess(Agent agent)
    {
        // Increase utility if projectile hits
        SuccessCount++;
        OnSuccessCallback?.Invoke();
        UpdateSuccessRate();
        int totalAttempts = SuccessCount + FailureCount;
        AgentLogger.Log(
            $"Action {Helpers.CleanName(name)} has succeeded. Attempts: {totalAttempts}. Success rate: {SuccessRate}, Feedback modifier: {FeedbackModifier}."
        );
    }

    public void UpdateSuccessRate()
    {
        int totalAttempts = SuccessCount + FailureCount;
        if (totalAttempts > 0)
        {
            SuccessRate = (float)SuccessCount / totalAttempts;
        }
    }
}
