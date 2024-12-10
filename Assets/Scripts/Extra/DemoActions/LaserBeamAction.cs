using System;
using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/LaserBeam")]
public class LaserBeamAction : AgentAction
{
    public GameObject laserPrefab;
    public float duration = 2f;
    public float laserDistance = 100f;

    [Range(0.0f, 1.0f)]
    public float accuracy = 1.0f;
    private Agent _agent;

    public override void Initialize(Agent agent)
    {
        base.Initialize(agent);
        _agent = agent;
    }

    public override void Execute(Transform firePoint, Vector3 direction)
    {
        ShootLaser(firePoint, _agent);
        AfterExecution();
    }

    private void ShootLaser(Transform firePoint, Agent agent)
    {
        Vector3 directionToTarget;

        PlayerMetrics playerMetrics = (PlayerMetrics)GameManager.Instance.playerAgent.Metrics;
        var nearestEnemy = playerMetrics.FindClosestEnemyToPlayer();
        directionToTarget = (nearestEnemy.position - _agent.firePoint.position).normalized;

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
        laserCollision.Initialize(agent, 100, true);

        Destroy(laser, duration);
    }
}
