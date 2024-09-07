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

        if (GetCooldownProgress() < 1.0f)
        {
            // If on cooldown, scaled by cooldown progress
            calculatedUtil *= GetCooldownProgress();
        }

        calculatedUtil *= baseUtility;
        //calculatedUtil *= Mathf.Max(1.0f - DecayFactor * 0.5f, MIN_UTILITY);
        //RestoreUtilityOverTime();

        if (calculatedUtil <= 0)
            Debug.LogWarning(
                $"[Frame {Time.frameCount}] Utility of {Helpers.CleanName(name)} is zero or negative, check parameters: Distance="
                    + distance
                    + ", CanSense="
                    + agent.PerceptionModule.CanSenseTarget
            );

        //Debug.Log("Utility calculated: " + calculatedUtil);
        //utilityScore = Mathf.Clamp(calculatedUtil, MIN_UTILITY, MAX_UTILITY);
        utilityScore = Mathf.Max(calculatedUtil, MIN_UTILITY);
    }

    public void HandleFailure(Agent agent)
    {
        // Decrease effectiveness when the projectile misses
        //agent.UtilityManager.FeedbackUtilityAdjustment(this, -effectivenessAdjustment);
        //OnFailureCallback?.Invoke();
        //Debug.Log("HaNDLED FAILURE");
    }

    public void HandleSuccess(Agent agent)
    {
        // Increase effectiveness when the projectile hits
        //agent.UtilityManager.FeedbackUtilityAdjustment(this, effectivenessAdjustment);
        //OnSuccessCallback?.Invoke();
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

    private void HandleCloseMiss(Agent agent)
    {
        //agent.actionUtilityManager.AdjustUtilityScore(this, -(effectivenessAdjustment / 2));
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
