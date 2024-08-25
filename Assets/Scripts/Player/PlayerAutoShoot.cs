using UnityEngine;

public class PlayerAutoShoot : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootingRate = 2.0f;
    public float projectileSpeed = 15f;

    private float shootingTimer;

    void Update()
    {
        shootingTimer -= Time.deltaTime;
        if (shootingTimer <= 0f)
        {
            ShootAtNearestEnemy();
            shootingTimer = shootingRate;
        }
    }

    void ShootAtNearestEnemy()
    {
        var nearestEnemy = Player.Instance.PlayerMetrics.FindClosestEnemyToPlayer();
        if (nearestEnemy != null)
        {
            var direction = nearestEnemy.transform.position - firePoint.position;
            Shoot(direction.normalized);
        }
    }

    void Shoot(Vector3 direction)
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(direction)
        );

        if (projectile.TryGetComponent<Projectile>(out var projectileComponent))
            projectileComponent.Initialize(direction, projectileSpeed, 10);
    }
}
