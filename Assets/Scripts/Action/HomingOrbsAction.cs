using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/HomingOrbs")]
public class HomingOrbsAction : AgentAction
{
    public GameObject orbPrefab;
    public int numberOfOrbs = 3;
    public float spreadAngle = 45f;
    SeeingModule seeingModule;
    ActionReadinessModule actionReadinessModule;

    public override void Initialize(Agent agent)
    {
        seeingModule = agent.GetModule<SeeingModule>();
        actionReadinessModule = agent.GetModule<ActionReadinessModule>();
        Debug.Assert(seeingModule != null, "SeeingModule is not set!");
    }

    public override bool CanExecute(Agent agent)
    {
        return seeingModule.canSeeTarget && Time.time - lastExecutedTime >= cooldownTime;
    }

    public override void ExecuteActionLoop(Transform firePoint, Agent agent)
    {
        if (seeingModule.canSeeTarget)
            agent.actionUtilityManager.AdjustUtilityScore(this, -10f * Time.deltaTime);
        else
        {
            ShootOrbs(firePoint);
            agent.actionUtilityManager.AdjustUtilityScore(this, 10f * Time.deltaTime);
        }

        lastExecutedTime = Time.time;
    }

    private void ShootOrbs(Transform firePoint)
    {
        for (int i = 0; i < numberOfOrbs; i++)
        {
            float angle = (i - numberOfOrbs / 2) * spreadAngle / numberOfOrbs;
            Quaternion rotation = Quaternion.Euler(0, angle, 0) * firePoint.rotation;
            GameObject orb = Instantiate(orbPrefab, firePoint.position, rotation);
            Destroy(orb, 8);
        }
    }
}
