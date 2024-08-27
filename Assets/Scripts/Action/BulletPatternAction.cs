using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/BulletPatternAction")]
public class BulletPatternAction : AgentAction
{
    public GameObject bulletPrefab;
    public int numberOfBullets = 10;
    public float bulletSpeed = 10f;
    public float angleIncrement = 10f;
    public float spawnRadius = 6f;
    private int currentSpiralAngleOffset = 0;

    public override void Initialize(Agent agent) { }

    public override void ExecuteLoop(Transform firePoint, Agent agent)
    {
        GeneratePattern(agent.transform);
        currentSpiralAngleOffset += (int)angleIncrement;
        lastExecutedTime = Time.time;
    }

    private void GeneratePattern(Transform firePoint)
    {
        // Evenly distribute bullets around circle
        float angleStep = 360f / numberOfBullets;
        float angle = currentSpiralAngleOffset;

        for (int i = 0; i < numberOfBullets; i++)
        {
            float angleInRadians = angle * Mathf.Deg2Rad;

            Vector3 spawnPosition =
                firePoint.position
                + new Vector3(
                    Mathf.Cos(angleInRadians) * spawnRadius,
                    0f,
                    Mathf.Sin(angleInRadians) * spawnRadius
                );

            Vector3 direction = (spawnPosition - firePoint.position).normalized;
            GameObject bullet = Instantiate(
                bulletPrefab,
                spawnPosition + new Vector3(0f, -4f, 0f),
                Quaternion.identity
            );
            bullet.GetComponent<Projectile>().Initialize(direction, bulletSpeed, 10);

            // Increment angle to evenly space bullets
            angle += angleStep;
        }
    }

    public override float CalculateUtility(Agent agent, AgentMetrics metrics)
    {
        //agent.ActionUtilityManager.NormalizeUtilityScores();
        utilityScore = 1.0f;
        return utilityScore;
    }

    public override bool CanExecute(Agent agent)
    {
        return !IsOnCooldown();
    }
}
