using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SenteAI/Actions/BulletPatternAction")]
public class BulletPatternAction : AgentAction
{
    public GameObject bulletPrefab;
    public int numberOfBullets = 12;
    public float bulletSpeed = 10f;
    public float angleIncrement = 10f;
    public float spawnRadius = 6f;
    protected float currentSpiralAngleOffset = 0;
    protected float heightOffset = 0f;
    protected const float PHI = 1.6180339887f; // (1 + Mathf.Sqrt(5)) / 2

    public override void Execute(Transform firePoint, Vector3 direction)
    {
        GeneratePattern(_agent.transform);
        AfterExecution();
    }

    protected void GeneratePattern(Transform firePoint)
    {
        float angleStep = 360f / numberOfBullets;
        float angle = currentSpiralAngleOffset;

        for (int i = 0; i < numberOfBullets; i++)
        {
            Vector3 spawnPosition =
                firePoint.position
                + new Vector3(Mathf.Cos(angle) * spawnRadius, 0f, Mathf.Sin(angle) * spawnRadius);

            Vector3 direction = (spawnPosition - firePoint.position).normalized;

            GameObject bullet = Instantiate(
                bulletPrefab,
                spawnPosition + new Vector3(0f, heightOffset, 0f),
                Quaternion.identity
            );

            bullet.GetComponent<Projectile>().SetParameters(_agent, direction, bulletSpeed, 10);

            currentSpiralAngleOffset += PHI * angleIncrement;
            angle += angleStep;
        }
    }
}
