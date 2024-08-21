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
    ActionReadinessModule actionReadinessModule;

    public override void Initialize(Agent agent)
    {
        seeingModule = agent.GetModule<SeeingModule>();
        actionReadinessModule = agent.GetModule<ActionReadinessModule>();
        Debug.Assert(seeingModule != null, "SeeingModule is not set!");
    }

    public override void ExecuteActionLoop(Transform firePoint, Agent agent)
    {
        ShootLaser(firePoint, agent);
        lastExecutedTime = Time.time;
    }

    public override bool CanExecute(Agent agent)
    {
        return seeingModule.canSeeTarget && Time.time - lastExecutedTime >= cooldownTime;
    }

    public override void UpdateUtilityLoop(Agent agent)
    {
        //float energyFactor =
        //energyBasedReadinessModule.curEnergy / energyBasedReadinessModule.maxEnergy;
        float healthFactor = agent.CurrentHealth / agent.MaxHealth;

        if (seeingModule.canSeeTarget)
        {
            float utility = CalculateUtility(agent.distanceToPlayer, healthFactor, 0.5f);
            agent.actionUtilityManager.AdjustUtilityScore(this, utility * Time.deltaTime);
        }
        else
        {
            agent.actionUtilityManager.AdjustUtilityScore(this, -10f * Time.deltaTime);
        }
    }

    private float CalculateUtility(float distance, float health, float energy)
    {
        return Mathf.Clamp01(1.0f - distance / 100f)
            * Mathf.Clamp01(health)
            * Mathf.Clamp01(energy)
            * Time.deltaTime;
    }

    private void ShootLaser(Transform firePoint, Agent agent)
    {
        Vector3 directionToTarget =
            Player.Instance.playerMetrics.PredictNextPositionUsingMomentum();

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
