using UnityEngine;

[CreateAssetMenu(menuName = "SenteAI/Actions/ShootAction")]
public class ShootAction : AgentAction
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10.0f;
    public int damage = 10;

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

        if (projectile.TryGetComponent<Projectile>(out var projectileComponent))
        {
            projectileComponent.SetParameters(_agent, direction, projectileSpeed, damage);
        }

        Destroy(projectile, 4f);
    }
}
