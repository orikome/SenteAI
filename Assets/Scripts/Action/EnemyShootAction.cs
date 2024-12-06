using System;
using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/EnemyShootAction")]
public class EnemyShootAction : ShootAction, IFeedbackAction
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
        if (_agent.Faction.Equals(Faction.Enemy))
        {
            PlayerMetrics playerMetrics = (PlayerMetrics)GameManager.Instance.playerAgent.Metrics;
            Vector3 predictedPlayerPosition = playerMetrics.PredictPositionDynamically();
            Vector3 directionToPlayer = predictedPlayerPosition - _agent.firePoint.position;

            if (!HasClearShot(firePoint, _agent))
                return;

            EnemyMetrics enemyMetrics = (EnemyMetrics)_agent.Metrics;
            // If distance is less than 30, directly shoot at player instead of predicting position
            if (enemyMetrics.DistanceToPlayer < 30f)
                directionToPlayer = GameManager.Instance.playerAgent.transform.position;
            ShootProjectile(firePoint, directionToPlayer);
            AfterExecution();
        }
        else
        {
            Vector3 directionToEnemy = _agent.Target.position - _agent.firePoint.position;

            if (!HasClearShot(firePoint, _agent))
                return;

            ShootProjectile(firePoint, directionToEnemy);
            AfterExecution();
        }
    }

    private bool HasClearShot(Transform firePoint, Agent agent)
    {
        if (agent.Faction.Equals(Faction.Ally))
        {
            return true;
        }

        PlayerMetrics playerMetrics = (PlayerMetrics)GameManager.Instance.playerAgent.Metrics;
        Vector3 predictedPlayerPosition = playerMetrics.PredictPositionDynamically();
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
        float distance;
        if (agent.CompareTag("Enemy"))
        {
            EnemyMetrics enemyMetrics = (EnemyMetrics)agent.Metrics;
            distance = enemyMetrics.DistanceToPlayer;
        }
        else
        {
            AllyMetrics allyMetrics = (AllyMetrics)agent.Metrics;
            distance = allyMetrics.DistanceToNearestEnemy;
        }
        float maxDistance = 100f;
        float CanSenseFactor = agent.GetModule<SenseModule>().CanSenseTarget ? 0.8f : MIN_UTILITY;
        float maxProjectileSpeed = 30f; // Fast projectile speed
        float speedFactor = Mathf.Clamp01(projectileSpeed / maxProjectileSpeed);

        // Weigh speed more for longer distances, because slower projectiles have less chance of hitting at range
        float distanceFactor = 1.0f - (distance / maxDistance);
        float speedDistanceFactor = distanceFactor * speedFactor;
        float calculatedUtil = speedDistanceFactor * 0.5f * CanSenseFactor;

        SetUtilityWithModifiers(calculatedUtil);
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
        DebugManager.Instance.Log(
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

    protected override void ShootProjectile(Transform firePoint, Vector3 direction)
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(direction)
        );

        Debug.DrawRay(firePoint.position, direction.normalized * 5f, Color.blue, 1f);

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
