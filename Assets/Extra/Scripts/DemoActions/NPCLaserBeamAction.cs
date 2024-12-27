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
    LayerMask obstacleLayerMask;
    public GameObject laserSparks;

    public override void Initialize(Agent agent)
    {
        base.Initialize(agent);

        obstacleLayerMask = OrikomeUtils.LayerMaskUtils.CreateMask("Wall");
    }

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
        Vector3 directionToTarget = agent.Target.transform.position - agent.firePoint.position;

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
        base.CanExecute(agent);

        return agent.GetModule<SenseModule>().CanSenseTarget
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
        Vector3 directionToTarget = (
            agent.Target.transform.position - firePoint.position
        ).normalized;

        Vector3 spawnPosition = _agent.transform.position;
        spawnPosition.y = 0.001f;

        directionToTarget.y = 0;

        // Spawn warning indicator
        GameObject obj = Instantiate(
            warningIndicator,
            spawnPosition,
            Helpers.GetYAxisLookRotation(directionToTarget)
        );
        obj.GetComponentInChildren<WarningIndicator>().Initialize(_agent);
        _agent.GetModule<NavMeshAgentModule>().PauseFor(3f);
        _agent.GetModule<Brain>().PauseFor(3f);
        yield return new WaitForSeconds(1f);

        // Spawn sparks
        GameObject sparks = Instantiate(
            laserSparks,
            _agent.firePoint.position,
            Quaternion.LookRotation(directionToTarget)
        );
        Destroy(sparks, 2f);

        GameObject laser = Instantiate(
            laserPrefab,
            firePoint.position,
            Quaternion.LookRotation(directionToTarget)
        );

        LineRenderer line = laser.GetComponent<LineRenderer>();
        line.SetPosition(0, firePoint.position);
        line.SetPosition(1, firePoint.position + (directionToTarget * laserDistance));

        // Add laser collider
        BoxCollider laserCollider = laser.AddComponent<BoxCollider>();
        laserCollider.isTrigger = true;
        laserCollider.center = new Vector3(0, 0, laserDistance / 2f);
        laserCollider.size = new Vector3(1.5f, 1.5f, laserDistance);
        LaserBehavior laserCollision = laser.AddComponent<LaserBehavior>();
        laserCollision.Initialize(agent, 100);
        laserCollision.OnHitCallback = () => HandleSuccess(agent);
        laserCollision.OnMissCallback = () => HandleFailure(agent);

        _agent.GetModule<NavMeshAgentModule>().PauseFor(2f);
        _agent.GetModule<Brain>().PauseFor(2f);
        Destroy(laser, duration);
        yield return new WaitForSeconds(duration);
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
