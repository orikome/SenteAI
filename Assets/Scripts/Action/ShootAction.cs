using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/ShootAction")]
public class ShootAction : AgentAction
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10.0f;
    public int damage = 10;
    private Agent _agent;

    public override void Initialize(Agent agent)
    {
        _agent = agent;
    }

    public override void Execute(Transform firePoint, Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            ShootProjectile(firePoint, direction.normalized);
            AfterExecution();
        }
    }

    protected virtual void ShootProjectile(Transform firePoint, Vector3 direction)
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(direction)
        );

        Debug.DrawRay(firePoint.position, direction * 5f, Color.blue, 1f);

        if (projectile.TryGetComponent<Projectile>(out var projectileComponent))
        {
            projectileComponent.SetParameters(_agent, direction, projectileSpeed, damage);
        }

        Destroy(projectile, 4f);
    }
}
