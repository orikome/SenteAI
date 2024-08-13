using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/LaserBeam")]
public class LaserBeamAction : AgentAction
{
    public GameObject laserPrefab;
    public float duration = 2f;
    public float laserDistance = 100f;

    [Range(0.0f, 1.0f)]
    public float accuracy = 1.0f;
    SeeingModule seeingModule;

    public override void Initialize(Agent agent)
    {
        seeingModule = agent.GetModule<SeeingModule>();
        Debug.Assert(seeingModule != null, "SeeingModule is not set!");
    }

    public override void ExecuteAction(Transform firePoint, Agent agent)
    {
        if (seeingModule.canSeeTarget)
        {
            ShootLaser(firePoint);
            agent.actionWeightManager.AdjustWeight(this, 0.1f);
        }
        else
        {
            agent.actionWeightManager.AdjustWeight(this, -1.0f);
            Debug.Log("Target not in sight!");
        }
    }

    public override void UpdateWeights(Agent agent)
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

    private void ShootLaser(Transform firePoint)
    {
        Vector3 directionToTarget = PredictionUtility.PredictPosition(
            firePoint.position,
            Player.Instance.transform,
            laserDistance,
            accuracy
        );

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
