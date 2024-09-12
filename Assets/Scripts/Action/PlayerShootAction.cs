using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/PlayerShootAction")]
public class PlayerShootAction : AgentAction
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10.0f;
    public int damage = 10;

    public override void Execute(Transform firePoint, Enemy agent)
    {
        Vector3 dir = agent.transform.forward;
        ShootProjectile(firePoint, dir, agent);
        AfterExecution();
    }

    void ShootProjectile(Transform firePoint, Vector3 direction, Enemy agent)
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(direction)
        );

        Debug.DrawRay(firePoint.position, direction.normalized * 5f, Color.blue, 1f);

        Projectile projectileComponent = projectile.GetComponent<Projectile>();
        projectileComponent.SetParameters(direction, projectileSpeed, damage);
        Destroy(projectile, 4f);
    }
}
