using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "SenteAI/Actions/NPCLaserBeam")]
public class NPCLaserBeamAction : LaserBeamAction, IFeedbackAction
{
    // Feedback interface
    public Action OnSuccessCallback { get; set; }
    public Action OnFailureCallback { get; set; }
    public int SuccessCount { get; set; } = 0;
    public int FailureCount { get; set; } = 0;
    public float SuccessRate { get; set; } = 1.0f;
    public float FeedbackModifier { get; set; } = 1.0f;
    public GameObject warningIndicator;

    public override void Execute(Transform firePoint, Vector3 direction)
    {
        if (!HasClearShot(firePoint, _agent))
            return;

        _agent.StartCoroutine(ShootLaser(firePoint, _agent));
        AfterExecution();
    }

    private bool HasClearShot(Transform firePoint, Agent agent)
    {
        Metrics targetMetrics = agent.Target.Metrics;
        Vector3 predictedTargetPosition = targetMetrics.PredictPosition();
        Vector3 directionToTarget = predictedTargetPosition - agent.firePoint.position;
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

    public override bool CanExecute(Agent agent)
    {
        return agent.GetModule<SenseModule>().CanSenseTarget
            && !IsOnCooldown()
            && ScaledUtilityScore > MIN_UTILITY
            && HasClearShot(agent.firePoint, agent);
    }

    public override void CalculateUtility(Agent agent)
    {
        Metrics metrics = agent.Metrics;
        float distance = metrics.DistanceToTarget;
        float maxDistance = 100f;
        float CanSenseFactor = agent.GetModule<SenseModule>().CanSenseTarget ? 0.8f : MIN_UTILITY;
        float distanceFactor = 1.0f - (distance / maxDistance);
        float calculatedUtil = distanceFactor * 0.5f * CanSenseFactor;

        SetUtilityWithModifiers(calculatedUtil);
    }

    private IEnumerator ShootLaser(Transform firePoint, Agent agent)
    {
        Metrics targetMetrics = agent.Target.Metrics;
        Vector3 directionToTarget = targetMetrics.PredictPosition();

        Vector3 spawnPosition = _agent.transform.position;
        spawnPosition.y = 0.001f;

        // Spawn warning indicator
        GameObject obj = Instantiate(
            warningIndicator,
            spawnPosition,
            Quaternion.LookRotation(directionToTarget)
        );
        obj.GetComponentInChildren<WarningIndicator>().Initialize(_agent);
        _agent.GetModule<NavMeshAgentModule>().PauseFor(1f);
        _agent.GetModule<Brain>().PauseFor(1f);
        // Wait for animation to reach impact frame
        yield return new WaitForSeconds(1f);

        GameObject laser = Instantiate(
            laserPrefab,
            firePoint.position,
            Quaternion.LookRotation(directionToTarget)
        );

        LineRenderer line = laser.GetComponent<LineRenderer>();
        line.SetPosition(0, firePoint.position);
        line.SetPosition(1, directionToTarget * laserDistance);

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
