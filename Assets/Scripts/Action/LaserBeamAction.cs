using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/LaserBeam")]
public class LaserBeamAction : AgentAction
{
    public GameObject laserPrefab;
    public float duration = 2f;
    public float laserDistance = 100f;

    [Range(0.0f, 1.0f)]
    public float accuracy = 1.0f;

    public override void Initialize(Agent agent) { }

    public override void ExecuteLoop(Transform firePoint, Agent agent)
    {
        ShootLaser(firePoint, agent);
        CalculateUtility(agent, agent.AgentMetrics);
        AddCooldown();
    }

    public override bool CanExecute(Agent agent)
    {
        return agent.PerceptionModule.CanSenseTarget && GetCooldownTimeRemaining() <= 0;
    }

    public override float CalculateUtility(Agent agent, AgentMetrics metrics)
    {
        if (agent.PerceptionModule.CanSenseTarget)
            return 0;

        float utility = CalcUtil(metrics.DistanceToPlayer, 0.5f, 0.5f);
        return utility;
    }

    private float CalcUtil(float distance, float health, float energy)
    {
        return Mathf.Clamp01(1.0f - distance / 100f)
            * Mathf.Clamp01(health)
            * Mathf.Clamp01(energy)
            * Time.deltaTime;
    }

    private void ShootLaser(Transform firePoint, Agent agent)
    {
        Vector3 directionToTarget =
            Player.Instance.PlayerMetrics.PredictNextPositionUsingMomentum();

        GameObject laser = Instantiate(
            laserPrefab,
            firePoint.position,
            Quaternion.LookRotation(directionToTarget)
        );

        LineRenderer line = laser.GetComponent<LineRenderer>();
        line.SetPosition(0, firePoint.position);
        line.SetPosition(1, firePoint.position + directionToTarget * laserDistance);
        Destroy(laser, duration);
    }
}
