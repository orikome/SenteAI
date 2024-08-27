using System;
using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/Shoot")]
public class ShootAction : AgentAction, IFeedbackAction
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10.0f;

    [Range(0.0f, 1.0f)]
    public float accuracy = 1.0f;
    float effectivenessAdjustment = 10f;
    public float closeMissThreshold = 1f;
    public int damage = 10;
    public Action OnSuccessCallback { get; set; }
    public Action OnFailureCallback { get; set; }

    public override bool CanExecute(Agent agent)
    {
        return agent.PerceptionModule.CanSenseTarget && GetCooldownTimeRemaining() <= 0;
    }

    public override void Initialize(Agent agent) { }

    public override void ExecuteLoop(Transform firePoint, Agent agent)
    {
        ShootProjectile(
            firePoint,
            Player.Instance.PlayerMetrics.PredictPositionDynamically() - agent.firePoint.position,
            agent
        );
        lastExecutedTime = Time.time;
        CalculateUtility(agent, agent.AgentMetrics);
    }

    public override float CalculateUtility(Agent agent, AgentMetrics metrics)
    {
        if (agent.PerceptionModule.CanSenseTarget)
            return 0.5f;

        float distance = agent.AgentMetrics.DistanceToPlayer;

        return Mathf.Clamp01(1.0f - distance / 100f)
            * Mathf.Clamp01(agent.AgentMetrics.HealthFactor)
            * Mathf.Clamp01(0.5f);
    }

    public void HandleFailure(Agent agent)
    {
        // Decrease effectiveness when the projectile misses
        //agent.actionUtilityManager.AdjustUtilityScore(this, -effectivenessAdjustment);
        OnFailureCallback?.Invoke();
        //Debug.Log("HaNDLED FAILURE");
    }

    public void HandleSuccess(Agent agent)
    {
        // Increase effectiveness when the projectile hits
        //agent.actionUtilityManager.AdjustUtilityScore(this, effectivenessAdjustment);
        OnSuccessCallback?.Invoke();
    }

    public void HandleMiss(Agent agent, float distanceToPlayer)
    {
        if (distanceToPlayer <= closeMissThreshold)
        {
            HandleCloseMiss(agent);
        }
        else
        {
            HandleFailure(agent);
        }
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
        projectileComponent.Initialize(direction, projectileSpeed, damage);

        if (projectileComponent != null)
        {
            projectileComponent.OnHitCallback = () => HandleSuccess(agent);
            projectileComponent.OnMissCallback = () => HandleFailure(agent);
        }
        Destroy(projectile, 4f);
    }
}
