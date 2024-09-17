using System;
using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/EnemyHomingOrbsAction")]
public class EnemyHomingOrbsAction : HomingOrbsAction, IFeedbackAction
{
    // Feedback interface
    public Action OnSuccessCallback { get; set; }
    public Action OnFailureCallback { get; set; }
    public int SuccessCount { get; set; } = 0;
    public int FailureCount { get; set; } = 0;
    public float SuccessRate { get; set; } = 1.0f;
    public float FeedbackModifier { get; set; } = 1.0f;
    private Enemy _enemy;

    public override void Initialize(Agent agent)
    {
        base.Initialize(agent);
        _enemy = (Enemy)agent;
    }

    public override void Execute(Transform firePoint, Vector3 direction)
    {
        ShootOrbs(firePoint);
        AfterExecution();
    }

    public override void CalculateUtility(Enemy agent)
    {
        float distance = _enemy.Metrics.DistanceToPlayer;
        float maxDistance = 100f;
        float CanSenseFactor = _enemy.PerceptionModule.CanSenseTarget ? 0.6f : 1f;
        float distanceFactor = 1.0f - (distance / maxDistance);
        float calculatedUtil = distanceFactor * 0.5f * CanSenseFactor;

        SetUtilityWithModifiers(calculatedUtil);
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
                orbComponent.SetParameters(false);
                orbComponent.OnHitCallback = () => HandleSuccess(_enemy);
                orbComponent.OnMissCallback = () => HandleFailure(_enemy);
            }
        }
    }

    public float ApplyFeedbackModifier(float utility, IFeedbackAction feedbackAction)
    {
        float modifiedUtility = utility;

        if (SuccessRate >= 0.5f)
            // Success rate is good, boost utility
            FeedbackModifier = Mathf.Lerp(1.0f, 1.5f, SuccessRate);
        else
            // Success rate is low, add penalty
            FeedbackModifier = Mathf.Lerp(0.5f, 1.0f, SuccessRate);

        modifiedUtility *= Mathf.Max(FeedbackModifier, MIN_UTILITY);

        return modifiedUtility;
    }

    public void HandleFailure(Enemy agent)
    {
        // Decrease utility if projectile misses
        FailureCount++;
        OnFailureCallback?.Invoke();
        UpdateSuccessRate();
        int totalAttempts = SuccessCount + FailureCount;
        DebugManager.Instance.Log(
            $"Action {Helpers.CleanName(name)} has failed. Attempts: {totalAttempts}. Success rate: {SuccessRate}, Feedback modifier: {FeedbackModifier}."
        );
    }

    public void HandleSuccess(Enemy agent)
    {
        // Increase utility if projectile hits
        SuccessCount++;
        OnSuccessCallback?.Invoke();
        UpdateSuccessRate();
        int totalAttempts = SuccessCount + FailureCount;
        DebugManager.Instance.Log(
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