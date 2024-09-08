using System;
using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/Shoot")]
public class ShootAction : AgentAction, IFeedbackAction
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10.0f;

    [Range(0.0f, 1.0f)]
    public float accuracy = 1.0f;
    public int damage = 10;
    public Action OnSuccessCallback { get; set; }
    public Action OnFailureCallback { get; set; }

    // Track success
    private int _successCount = 0;
    private int _failureCount = 0;
    private float _successRate = 1.0f;
    private float _feedbackModifier = 0.0f;

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

    public override void CalculateUtility(Agent agent)
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

        if (_successRate >= 0.5f)
            // Success rate is good, boost utility
            _feedbackModifier = Mathf.Lerp(1.0f, 1.5f, _successRate);
        else
            // Success rate is low, add penalty
            _feedbackModifier = Mathf.Lerp(0.5f, 1.0f, _successRate);

        // Add feedback modifier
        calculatedUtil *= Mathf.Max(_feedbackModifier, MIN_UTILITY);

        SetCalculatedUtility(calculatedUtil);
    }

    public void HandleFailure(Agent agent)
    {
        // Decrease utility if projectile misses
        _failureCount++;
        UpdateSuccessRate();
        OnFailureCallback?.Invoke();
    }

    public void HandleSuccess(Agent agent)
    {
        // Increase utility if projectile hits
        _successCount++;
        UpdateSuccessRate();
        OnSuccessCallback?.Invoke();
    }

    public void HandleMiss(Agent agent, float distanceToPlayer)
    {
        // Calculate when trajectory is past player and cannot hit
    }

    public void UpdateSuccessRate()
    {
        int totalAttempts = _successCount + _failureCount;
        if (totalAttempts > 0)
        {
            _successRate = (float)_successCount / totalAttempts;
        }
        DebugManager.Instance.Log(
            $"Action {Helpers.CleanName(name)} has success rate of: {_successRate}, which adds a modifier of: {_feedbackModifier}."
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
