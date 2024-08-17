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
    EnergyBasedReadinessModule energyBasedReadinessModule;

    public override void Initialize(Agent agent)
    {
        seeingModule = agent.GetModule<SeeingModule>();
        energyBasedReadinessModule = agent.GetModule<EnergyBasedReadinessModule>();
        Debug.Assert(seeingModule != null, "SeeingModule is not set!");
    }

    public override void ExecuteAction(Transform firePoint, Agent agent)
    {
        if (seeingModule.canSeeTarget)
        {
            ShootLaser(firePoint);
            agent.actionWeightManager.AdjustWeight(this, 10f * Time.deltaTime);
        }
        else
        {
            agent.actionWeightManager.AdjustWeight(this, -10f * Time.deltaTime);
            Debug.Log("Target not in sight!");
        }
    }

    public override void UpdateWeights(Agent agent)
    {
        float distanceToPlayer = OrikomeUtils.GeneralUtils.GetDistanceSquared(
            agent.transform.position,
            Player.Instance.transform.position
        );
        float energyFactor =
            energyBasedReadinessModule.curEnergy / energyBasedReadinessModule.maxEnergy;
        float healthFactor = agent.CurrentHealth / agent.MaxHealth;

        if (seeingModule.canSeeTarget)
        {
            float utility = CalculateUtility(distanceToPlayer, healthFactor, energyFactor * 3);
            agent.actionWeightManager.AdjustWeight(this, utility);
        }
        else
        {
            agent.actionWeightManager.AdjustWeight(this, -10f * Time.deltaTime);
        }
    }

    private float CalculateUtility(float distance, float health, float energy)
    {
        return Mathf.Clamp01(1.0f - distance / 100f)
            * Mathf.Clamp01(health)
            * Mathf.Clamp01(energy)
            * Time.deltaTime;
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
