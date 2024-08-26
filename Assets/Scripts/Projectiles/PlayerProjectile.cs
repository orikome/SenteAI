using UnityEngine;

public class PlayerProjectile : Projectile
{
    protected override void Start()
    {
        base.Start();
        _collisionMask = LayerMask.GetMask("Enemy");
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (OrikomeUtils.LayerMaskUtils.IsLayerInMask(collision.gameObject.layer, _collisionMask))
        {
            collision.transform.root.gameObject.TryGetComponent<Agent>(out var agent);
            agent.GetModule<HealthModule>().TakeDamage(10);
            Helpers.SpawnParticles(transform.position, Color.white);
            Destroy(gameObject);
        }
        else
        {
            Helpers.SpawnParticles(transform.position, Color.white);
            Destroy(gameObject);
        }
    }
}
