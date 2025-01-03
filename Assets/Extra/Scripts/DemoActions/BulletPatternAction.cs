using UnityEngine;

[CreateAssetMenu(menuName = "SenteAI/Actions/BulletPatternAction")]
public class BulletPatternAction : AgentAction
{
    public GameObject bulletPrefab;
    public int numberOfBullets = 12;
    public float bulletSpeed = 10f;
    public float angleIncrement = 10f;
    public float spawnRadius = 6f;
    protected int currentSpiralAngleOffset = 0;
    protected float heightOffset = 0f;

    public override void Execute(Transform firePoint, Vector3 direction)
    {
        GeneratePattern(_agent.transform);
        currentSpiralAngleOffset += (int)angleIncrement;
        AfterExecution();
    }

    protected void GeneratePattern(Transform firePoint)
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
                spawnPosition + new Vector3(0f, heightOffset, 0f),
                Quaternion.identity
            );
            bullet.GetComponent<Projectile>().SetParameters(_agent, direction, bulletSpeed, 10);

            // Increment angle to evenly space bullets
            angle += angleStep;
        }
    }
}
