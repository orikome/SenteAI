using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/PlayerShootAction")]
public class PlayerShootAction : AgentAction
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10.0f;
    public int damage = 10;
    private Agent _agent;

    public override void Initialize(Agent agent)
    {
        _agent = agent;
        Debug.Log("PlayerShootAction initialized for " + agent.name);
    }

    public override void Execute(Transform firePoint)
    {
        var nearestEnemy = Player.Instance.Metrics.FindClosestEnemyToPlayer();

        if (nearestEnemy != null)
        {
            var direction = nearestEnemy.position - firePoint.position;
            ShootProjectile(firePoint, direction.normalized);
        }

        AfterExecution();
        Debug.Log("PlayerShootAction executed.");
    }

    private void ShootProjectile(Transform firePoint, Vector3 direction)
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(direction)
        );

        Debug.DrawRay(firePoint.position, direction * 5f, Color.blue, 1f);

        if (projectile.TryGetComponent<Projectile>(out var projectileComponent))
        {
            projectileComponent.SetParameters(direction, projectileSpeed, damage);
        }

        Destroy(projectile, 4f);
    }
}
