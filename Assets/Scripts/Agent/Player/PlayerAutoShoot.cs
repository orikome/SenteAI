using UnityEngine;

public class PlayerAutoShoot : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootingRate = 2.0f;
    public float projectileSpeed = 15f;
    private float _shootingTimer;

    void Update()
    {
        _shootingTimer -= Time.deltaTime;
        if (_shootingTimer <= 0f)
        {
            ShootAtNearestEnemy();
            _shootingTimer = shootingRate;
        }
    }

    void ShootAtNearestEnemy()
    {
        var nearestEnemy = Player.Instance.Metrics.FindClosestEnemyToPlayer();
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
            projectileComponent.SetParameters(direction, projectileSpeed, 10);
    }
}
