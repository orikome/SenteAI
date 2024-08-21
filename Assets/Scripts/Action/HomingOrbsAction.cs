using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/HomingOrbs")]
public class HomingOrbsAction : AgentAction
{
    public GameObject orbPrefab;
    public int numberOfOrbs = 3;
    public float spreadAngle = 45f;

    public override void Initialize(Agent agent) { }

    public override bool CanExecute(Agent agent)
    {
        return agent.perceptionModule.CanSenseTarget && GetCooldownTimeRemaining() <= 0;
    }

    public override void ExecuteActionLoop(Transform firePoint, Agent agent)
    {
        if (agent.perceptionModule.CanSenseTarget)
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
