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
    private Agent _enemy;

    public override void Initialize(Agent agent)
    {
        base.Initialize(agent);
        _enemy = (Agent)agent;
    }

    public override void Execute(Transform firePoint, Vector3 direction)
    {
        PlayerMetrics playerMetrics = (PlayerMetrics)GameManager.Instance.playerAgent.Metrics;
        Vector3 predictedPlayerPosition = playerMetrics.PredictPositionDynamically();
        Vector3 directionToPlayer = predictedPlayerPosition - _enemy.firePoint.position;

        if (!HasClearShot(firePoint, _enemy))
            return;

        EnemyMetrics enemyMetrics = (EnemyMetrics)_enemy.Metrics;
        // If distance is less than 30, directly shoot at player instead of predicting position
        if (enemyMetrics.DistanceToPlayer < 30f)
            directionToPlayer = GameManager.Instance.playerAgent.transform.position;
        ShootProjectile(firePoint, directionToPlayer);
        AfterExecution();
    }

    private bool HasClearShot(Transform firePoint, Agent agent)
    {
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
        EnemyMetrics enemyMetrics = (EnemyMetrics)_enemy.Metrics;
        float distance = enemyMetrics.DistanceToPlayer;
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
        float modifiedUtility = utility;
        int totalAttempts = SuccessCount + FailureCount;

        if (SuccessRate >= 0.5f && totalAttempts != 0)
            // Success rate is good, boost utility
            FeedbackModifier = Mathf.Lerp(1.0f, 1.5f, SuccessRate);
        else
            // Success rate is low, add penalty
            FeedbackModifier = Mathf.Lerp(0.5f, 1.0f, SuccessRate);

        modifiedUtility *= Mathf.Max(FeedbackModifier, MIN_UTILITY);

        return modifiedUtility;
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
        projectileComponent.SetParameters(direction, projectileSpeed, damage);

        if (projectileComponent != null)
        {
            projectileComponent.OnHitCallback = () => HandleSuccess(_enemy);
            projectileComponent.OnMissCallback = () => HandleFailure(_enemy);
        }
        Destroy(projectile, 4f);
    }
}
