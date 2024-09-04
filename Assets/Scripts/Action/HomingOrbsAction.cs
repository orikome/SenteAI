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
