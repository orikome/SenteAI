using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SenteAI/Actions/NPCShootAction")]
public class NPCShootAction : ShootAction, IFeedbackAction
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
        Metrics targetMetrics = _agent.Target.Metrics;
        Vector3 predictedTargetPosition = targetMetrics.PredictPosition();
        Vector3 directionToTarget = predictedTargetPosition - firePoint.position;

        if (!HasClearShot(firePoint))
            return;

        ShootProjectile(firePoint, directionToTarget);
        AfterExecution();
    }

    private bool HasClearShot(Transform firePoint)
    {
        Metrics targetMetrics = _agent.Target.Metrics;
        Vector3 predictedTargetPosition = targetMetrics.PredictPosition();
        Vector3 directionToTarget = predictedTargetPosition - _agent.firePoint.position;
        LayerMask obstacleLayerMask = OrikomeUtils.LayerMaskUtils.CreateMask("Wall");

        if (Physics.Raycast(firePoint.position, directionToTarget, out RaycastHit hit))
        {
            if (
                OrikomeUtils.LayerMaskUtils.IsLayerInMask(
                    hit.transform.gameObject.layer,
                    obstacleLayerMask
                )
            )
            {
                // Obstacle detected, return false
                return false;
            }
        }

        // No obstacles, clear shot
        return true;
    }

    public override void CalculateUtility(Agent agent)
    {
        float utility = new UtilityBuilder()
            .WithDistance(agent.Metrics.DistanceToTarget, 60f, UtilityType.Linear)
            .WithSensing(agent.GetModule<SenseModule>().CanSenseTarget)
            .WithProjectileStats(projectileSpeed)
            .Build();

        SetUtilityWithModifiers(utility);
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
        if (agent == null)
            return;

        // Decrease utility if projectile misses
        FailureCount++;
        OnFailureCallback?.Invoke();
        UpdateSuccessRate();
        int totalAttempts = SuccessCount + FailureCount;
        AgentLogger.Log(
            $"Action {Helpers.CleanName(name)} has failed. Attempts: {totalAttempts}. Success rate: {SuccessRate}, Feedback modifier: {FeedbackModifier}.",
            _agent.gameObject
        );
    }

    public void HandleSuccess(Agent agent)
    {
        if (agent == null)
            return;

        // Increase utility if projectile hits
        SuccessCount++;
        OnSuccessCallback?.Invoke();
        UpdateSuccessRate();
        int totalAttempts = SuccessCount + FailureCount;
        AgentLogger.Log(
            $"Action {Helpers.CleanName(name)} has succeeded. Attempts: {totalAttempts}. Success rate: {SuccessRate}, Feedback modifier: {FeedbackModifier}.",
            _agent.gameObject
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

    protected override void ShootProjectile(Transform firePoint, Vector3 direction)
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(direction)
        );

        //Debug.DrawRay(firePoint.position, direction.normalized * 5f, Color.blue, 1f);

        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        projectileComponent.SetParameters(_agent, direction, projectileSpeed, damage);

        if (projectileComponent != null)
        {
            projectileComponent.OnHitCallback = () => HandleSuccess(_agent);
            projectileComponent.OnMissCallback = () => HandleFailure(_agent);
        }
        Destroy(projectile, 4f);
    }
}
