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
        return agent.PerceptionModule.CanSenseTarget && GetCooldownTimeRemaining() <= 0;
    }

    public override void ExecuteLoop(Transform firePoint, Agent agent)
    {
        ShootOrbs(firePoint);
        AddCooldown();
    }

    public override void CalculateUtility(Agent agent, AgentMetrics metrics)
    {
        float distance = agent.Metrics.DistanceToPlayer;
        float maxDistance = 100f;
        float CanSenseFactor = agent.PerceptionModule.CanSenseTarget ? 0.6f : 1f;
        float distanceFactor = 1.0f - (distance / maxDistance);
        float calculatedUtil = distanceFactor * 0.5f * CanSenseFactor;

        if (calculatedUtil <= 0)
            Debug.LogError(
                "Utility is zero or negative, check parameters: Distance="
                    + distance
                    + ", CanSense="
                    + agent.PerceptionModule.CanSenseTarget
            );

        //Debug.Log("Utility calculated: " + calculatedUtil);
        utilityScore = calculatedUtil;
    }

    private void ShootOrbs(Transform firePoint)
    {
        float distanceBetweenOrbs = 3.0f;
        Vector3 rightOffset = firePoint.right * distanceBetweenOrbs;

        for (int i = 0; i < numberOfOrbs; i++)
        {
            float offset = (i - (numberOfOrbs - 1) / 2.0f) * distanceBetweenOrbs;
            Vector3 spawnPosition = firePoint.position + rightOffset * offset;

            float angle = (i - numberOfOrbs / 2) * spreadAngle / numberOfOrbs;
            Quaternion rotation = Quaternion.Euler(0, angle, 0) * firePoint.rotation;
            GameObject orb = Instantiate(orbPrefab, spawnPosition, rotation);
        }
    }
}
