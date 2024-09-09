using System;
using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/LaserBeam")]
public class LaserBeamAction : AgentAction, IFeedbackAction
{
    public GameObject laserPrefab;
    public float duration = 2f;
    public float laserDistance = 100f;

    [Range(0.0f, 1.0f)]
    public float accuracy = 1.0f;

    // Feedback interface
    public Action OnSuccessCallback { get; set; }
    public Action OnFailureCallback { get; set; }
    public int SuccessCount { get; set; } = 0;
    public int FailureCount { get; set; } = 0;
    public float SuccessRate { get; set; } = 1.0f;
    public float FeedbackModifier { get; set; } = 1.0f;

    public override void Execute(Transform firePoint, Agent agent)
    {
        if (!HasClearShot(firePoint, agent))
            return;

        ShootLaser(firePoint, agent);
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

    public override bool CanExecute(Agent agent)
    {
        return agent.PerceptionModule.CanSenseTarget
            && !IsOnCooldown()
            && utilityScore > MIN_UTILITY
            && HasClearShot(agent.firePoint, agent);
    }

    public override void CalculateUtility(Agent agent)
    {
        float distance = agent.Metrics.DistanceToPlayer;
        float maxDistance = 100f;
        float CanSenseFactor = agent.PerceptionModule.CanSenseTarget ? 0.8f : MIN_UTILITY;
        float distanceFactor = 1.0f - (distance / maxDistance);
        float calculatedUtil = distanceFactor * 0.5f * CanSenseFactor;

        SetUtilityWithModifiers(calculatedUtil);
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

    private void ShootLaser(Transform firePoint, Agent agent)
    {
        Vector3 directionToTarget = Player.Instance.Metrics.PredictNextPositionUsingMomentum();

        GameObject laser = Instantiate(
            laserPrefab,
            firePoint.position,
            Quaternion.LookRotation(directionToTarget)
        );

        LineRenderer line = laser.GetComponent<LineRenderer>();
        line.SetPosition(0, firePoint.position);
        line.SetPosition(1, firePoint.position + directionToTarget * laserDistance);

        // Add laser collider
        BoxCollider laserCollider = laser.AddComponent<BoxCollider>();
        laserCollider.isTrigger = true;
        laserCollider.center = new Vector3(0, 0, laserDistance / 2f);
        laserCollider.size = new Vector3(1.5f, 1.5f, laserDistance);
        LaserBehavior laserCollision = laser.AddComponent<LaserBehavior>();
        laserCollision.Initialize(agent, 100);
        laserCollision.OnHitCallback = () => HandleSuccess(agent);
        laserCollision.OnMissCallback = () => HandleFailure(agent);

        Destroy(laser, duration);
    }

    public void HandleSuccess(Agent agent)
    {
        // Increase utility if projectile hits
        SuccessCount++;
        UpdateSuccessRate();
        OnSuccessCallback?.Invoke();
    }

    public void HandleFailure(Agent agent)
    {
        // Decrease utility if projectile misses
        FailureCount++;
        UpdateSuccessRate();
        OnFailureCallback?.Invoke();
    }

    public void UpdateSuccessRate()
    {
        int totalAttempts = SuccessCount + FailureCount;
        if (totalAttempts > 0)
        {
            SuccessRate = (float)SuccessCount / totalAttempts;
        }
        DebugManager.Instance.Log(
            $"Action {Helpers.CleanName(name)} has success rate of: {SuccessRate}, which adds a modifier of: {FeedbackModifier}."
        );
    }

    public void HandleMiss(Agent agent, float distanceToPlayer)
    {
        throw new NotImplementedException();
    }
}
