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
    SeeingModule seeingModule;

    public override void Initialize(Agent agent)
    {
        seeingModule = agent.GetModule<SeeingModule>();
    }

    public override void ExecuteActionLoop(Transform firePoint, Agent agent)
    {
        if (seeingModule.canSeeTarget)
        {
            ShootProjectile(
                firePoint,
                Player.Instance.playerMetrics.PredictPositionDynamically()
                    - agent.firePoint.position,
                agent
            );
        }
    }

    public override void UpdateUtilityLoop(Agent agent)
    {
        float healthFactor = agent.CurrentHealth / agent.MaxHealth;

        if (seeingModule.canSeeTarget)
        {
            float utility = CalculateUtilityScore(agent.distanceToPlayer, healthFactor, 0.5f);
            agent.actionUtilityManager.AdjustUtilityScore(this, utility * Time.deltaTime);
        }
        else
        {
            agent.actionUtilityManager.AdjustUtilityScore(this, -10f * Time.deltaTime);
        }
    }

    private float CalculateUtilityScore(float distance, float health, float energy)
    {
        return Mathf.Clamp01(1.0f - distance / 100f)
            * Mathf.Clamp01(health)
            * Mathf.Clamp01(energy)
            * Time.deltaTime;
    }

    public void HandleFailure(Agent agent)
    {
        // Decrease effectiveness when the projectile misses
        agent.actionUtilityManager.AdjustUtilityScore(this, -effectivenessAdjustment);
        OnFailureCallback?.Invoke();
        //Debug.Log("HaNDLED FAILURE");
    }

    public void HandleSuccess(Agent agent)
    {
        // Increase effectiveness when the projectile hits
        agent.actionUtilityManager.AdjustUtilityScore(this, effectivenessAdjustment);
        OnSuccessCallback?.Invoke();
        Debug.Log("Hit player");
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
        agent.actionUtilityManager.AdjustUtilityScore(this, -(effectivenessAdjustment / 2));
    }

    void ShootProjectile(Transform firePoint, Vector3 direction, Agent agent)
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(direction)
        );

        Debug.DrawRay(firePoint.position, direction * 100, Color.blue, 1f);

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
