using System;
using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/Shoot")]
public class ShootAction : AgentAction, IFeedbackAction
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10.0f;

    [Range(0.0f, 1.0f)]
    public float accuracy = 1.0f;
    float effectivenessAdjustment = 0.5f;
    public float closeMissThreshold = 1f;

    Transform target;

    public Action OnSuccessCallback { get; set; }
    public Action OnFailureCallback { get; set; }

    public override void ExecuteAction(Transform firePoint, Agent agent)
    {
        var seeingModule = agent.GetModule<SeeingModule>();
        if (seeingModule != null && seeingModule.canSeeTarget)
        {
            target = Player.Instance.transform;
            Vector3 aimDirection = PredictionUtility.PredictPosition(
                firePoint.position,
                target,
                projectileSpeed,
                accuracy
            );
            ShootProjectile(firePoint, aimDirection, agent);
            agent.actionWeightManager.AdjustWeight(this, 0.1f);
        }
        else
        {
            agent.actionWeightManager.AdjustWeight(this, -1f);
        }
    }

    public void HandleFailure(Agent agent)
    {
        // Decrease effectiveness when the projectile misses
        agent.actionWeightManager.AdjustWeight(this, -(effectivenessAdjustment / 4));
        OnFailureCallback?.Invoke();
        //Debug.Log("HaNDLED FAILURE");
    }

    public void HandleSuccess(Agent agent)
    {
        // Increase effectiveness when the projectile hits
        agent.actionWeightManager.AdjustWeight(this, effectivenessAdjustment);
        OnSuccessCallback?.Invoke();
        //Debug.Log("HaNDLED SUCCESS");
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
        SeeingModule seeingModule = agent.GetModule<SeeingModule>();
        if (seeingModule != null)
        {
            if (seeingModule.canSeeTarget)
            {
                agent.actionWeightManager.AdjustWeight(this, 0.1f);
            }
            else
            {
                agent.actionWeightManager.AdjustWeight(this, -0.1f);
            }
        }
    }

    void ShootProjectile(Transform firePoint, Vector3 direction, Agent agent)
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(direction)
        );
        projectile.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.OnHitCallback = () => HandleSuccess(agent);
            projectileComponent.OnMissCallback = () => HandleFailure(agent);
        }
        Destroy(projectile, 4f);
    }
}
