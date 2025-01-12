using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SenteAI/Actions/BulletPatternAction")]
public class BulletPatternAction : AgentAction
{
    public GameObject bulletPrefab;
    public BulletPattern patternType;
    public int numberOfBullets = 12;
    public float bulletSpeed = 10f;
    public float angleIncrement = 10f;
    public float spawnRadius = 6f;
    protected float currentSpiralAngleOffset = 0;
    protected float heightOffset = 0f;
    protected const float PHI = 1.6180339887f; // (1 + Mathf.Sqrt(5)) / 2
    public float oscillationSpeed = 2f;
    public float oscillationHeight = 1f;

    public enum BulletPattern
    {
        Spiral,
        DNAHelix,
        Mandala,
    }

    public override void Execute(Transform firePoint, Vector3 direction)
    {
        switch (patternType)
        {
            case BulletPattern.Spiral:
                GenerateSpiralPattern(firePoint);
                break;
            case BulletPattern.DNAHelix:
                GenerateDNAPattern(firePoint);
                break;
            case BulletPattern.Mandala:
                GenerateMandalaPattern(firePoint);
                break;
        }
        AfterExecution();
    }

    protected void GenerateSpiralPattern(Transform firePoint)
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

    protected void GenerateDNAPattern(Transform firePoint)
    {
        float angleStep = 360f / numberOfBullets;
        float time = Time.time * oscillationSpeed;

        for (int i = 0; i < numberOfBullets; i++)
        {
            for (int helix = 0; helix < 2; helix++)
            {
                float angle = currentSpiralAngleOffset + (helix * 180f);
                float heightWave = Mathf.Sin(time + i * 0.5f) * oscillationHeight;

                Vector3 spawnPosition =
                    firePoint.position
                    + new Vector3(
                        Mathf.Cos(angle * Mathf.Deg2Rad) * spawnRadius,
                        heightWave,
                        Mathf.Sin(angle * Mathf.Deg2Rad) * spawnRadius
                    );

                Vector3 direction = (spawnPosition - firePoint.position).normalized;

                SpawnBullet(spawnPosition, direction);
            }
            currentSpiralAngleOffset += angleStep;
        }
    }

    protected void GenerateMandalaPattern(Transform firePoint)
    {
        int rings = 3;
        for (int ring = 0; ring < rings; ring++)
        {
            float ringRadius = spawnRadius * (ring + 1) / rings;
            int bulletsInRing = numberOfBullets * ring;

            for (int i = 0; i < bulletsInRing; i++)
            {
                float angle = i * (360f / bulletsInRing) + (ring * currentSpiralAngleOffset);
                Vector3 spawnPosition =
                    firePoint.position
                    + new Vector3(
                        Mathf.Cos(angle * Mathf.Deg2Rad) * ringRadius,
                        heightOffset,
                        Mathf.Sin(angle * Mathf.Deg2Rad) * ringRadius
                    );

                Vector3 direction = (spawnPosition - firePoint.position).normalized;
                GameObject bullet = Instantiate(
                    bulletPrefab,
                    spawnPosition + new Vector3(0, 25f, 0),
                    Quaternion.identity
                );
                bullet
                    .GetComponent<Projectile>()
                    .SetParameters(_agent, direction, bulletSpeed * (1 + ring / 2f), 10);
            }
        }
        currentSpiralAngleOffset += angleIncrement;
    }

    private GameObject SpawnBullet(Vector3 position, Vector3 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, position, Quaternion.identity);
        bullet.GetComponent<Projectile>().SetParameters(_agent, direction, bulletSpeed, 10);
        return bullet;
    }
}
