using System;
using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/Shoot")]
public class ShootAction : AgentAction, IFeedbackAction
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10.0f;

    [Range(0.0f, 1.0f)]
    public float accuracy = 1.0f;
    float effectivenessAdjustment = 20f;
    public float closeMissThreshold = 1f;
    public int damage = 10;
    public Action OnSuccessCallback { get; set; }
    public Action OnFailureCallback { get; set; }

    // Track success
    private int successCount = 0;
    private int failureCount = 0;
    private float successRate = 1.0f;
    private float historicalPenalty = 0.0f;
    private float historicalPenaltyWeight = 0.5f;

    public override void Execute(Transform firePoint, Agent agent)
    {
        Vector3 predictedPlayerPosition = Player.Instance.Metrics.PredictPositionDynamically();
        Vector3 directionToPlayer = predictedPlayerPosition - agent.firePoint.position;

        if (!HasClearShot(firePoint, agent))
            return;

        // If distance is less than 30, directly shoot at player instead of predicting position
        if (agent.Metrics.DistanceToPlayer < 30f)
            directionToPlayer = Player.Instance.transform.position;
        ShootProjectile(firePoint, directionToPlayer, agent);
        AfterExecution();
    }

    private bool HasClearShot(Transform firePoint, Agent agent)
    {
        Vector3 predictedPlayerPosition = Player.Instance.Metrics.PredictPositionDynamically();
        Vector3 directionToPlayer = predictedPlayerPosition - agent.firePoint.position;
        LayerMask obstacleLayerMask = OrikomeUtils.LayerMaskUtils.CreateMask("Wall");

        if (Physics.Raycast(firePoint.position, directionToPlayer, out RaycastHit hit))
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

    public override void CalculateUtility(Agent agent, AgentMetrics metrics)
    {
        float distance = agent.Metrics.DistanceToPlayer;
        float maxDistance = 100f;
        float CanSenseFactor = agent.PerceptionModule.CanSenseTarget ? 0.8f : MIN_UTILITY;
        float maxProjectileSpeed = 30f; // Fast projectile speed
        float speedFactor = Mathf.Clamp01(projectileSpeed / maxProjectileSpeed);

        // Weigh speed more for longer distances, because slower projectiles have less chance of hitting at range
        float distanceFactor = 1.0f - (distance / maxDistance);
        float speedDistanceFactor = distanceFactor * speedFactor;
        float calculatedUtil = speedDistanceFactor * 0.5f * CanSenseFactor;

        // Apply penalty based on feedback
        historicalPenalty = Mathf.Lerp(0.2f, 1.0f, successRate) * historicalPenaltyWeight;
        calculatedUtil *= Mathf.Max(historicalPenalty, MIN_UTILITY);

        SetCalculatedUtility(calculatedUtil);
    }

    public void HandleFailure(Agent agent)
    {
        // Decrease utility if projectile misses
        failureCount++;
        UpdateSuccessRate();
        OnFailureCallback?.Invoke();
    }

    public void HandleSuccess(Agent agent)
    {
        // Increase utility if projectile hits
        successCount++;
        UpdateSuccessRate();
        OnSuccessCallback?.Invoke();
    }

    public void HandleMiss(Agent agent, float distanceToPlayer)
    {
        // if (distanceToPlayer <= closeMissThreshold)
        // {
        //     HandleCloseMiss(agent);
        // }
        // else
        // {
        //     HandleFailure(agent);
        // }
    }

    public void UpdateSuccessRate()
    {
        int totalAttempts = successCount + failureCount;
        if (totalAttempts > 0)
        {
            successRate = (float)successCount / totalAttempts;
        }
        DebugManager.Instance.Log(
            $"Action {Helpers.CleanName(name)} has success rate of: {successRate}, which adds a penalty of: {1.0f - historicalPenalty}."
        );
    }

    void ShootProjectile(Transform firePoint, Vector3 direction, Agent agent)
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(direction)
        );

        Debug.DrawRay(firePoint.position, direction.normalized * 5f, Color.blue, 1f);

        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        projectileComponent.SetParameters(direction, projectileSpeed, damage);

        if (projectileComponent != null)
        {
            projectileComponent.OnHitCallback = () => HandleSuccess(agent);
            projectileComponent.OnMissCallback = () => HandleFailure(agent);
        }
        Destroy(projectile, 4f);
    }
}
