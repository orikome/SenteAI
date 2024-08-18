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

    Transform target;

    public Action OnSuccessCallback { get; set; }
    public Action OnFailureCallback { get; set; }
    SeeingModule seeingModule;

    public override void Initialize(Agent agent)
    {
        seeingModule = agent.GetModule<SeeingModule>();
        target = Player.Instance.transform;
    }

    public override void ExecuteAction(Transform firePoint, Agent agent)
    {
        if (seeingModule.canSeeTarget)
        {
            Vector3 aimDirection = Helpers.PredictPosition(
                firePoint.position,
                target,
                projectileSpeed,
                accuracy
            );
            ShootProjectile(firePoint, aimDirection, agent);
            agent.actionWeightManager.AdjustWeight(this, 10f * Time.deltaTime);
        }
        else
        {
            agent.actionWeightManager.AdjustWeight(this, -10f * Time.deltaTime);
        }
    }

    public void HandleFailure(Agent agent)
    {
        // Decrease effectiveness when the projectile misses
        agent.actionWeightManager.AdjustWeight(this, -effectivenessAdjustment);
        OnFailureCallback?.Invoke();
        //Debug.Log("HaNDLED FAILURE");
    }

    public void HandleSuccess(Agent agent)
    {
        // Increase effectiveness when the projectile hits
        agent.actionWeightManager.AdjustWeight(this, effectivenessAdjustment);
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
        agent.actionWeightManager.AdjustWeight(this, -(effectivenessAdjustment / 2));
    }

    public override void UpdateWeights(Agent agent)
    {
        if (seeingModule.canSeeTarget)
        {
            agent.actionWeightManager.AdjustWeight(this, 10f * Time.deltaTime);
        }
        else
        {
            agent.actionWeightManager.AdjustWeight(this, -10f * Time.deltaTime);
        }
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
