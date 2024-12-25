using UnityEngine;

public class PlayerProjectile : Projectile
{
    protected override void Start()
    {
        base.Start();
        _collisionMask = LayerMask.GetMask("Enemy");
        SetColor(Color.blue);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (!_agent)
            return;

        if (OrikomeUtils.LayerMaskUtils.IsLayerInMask(collision.gameObject.layer, _collisionMask))
        {
            collision.transform.gameObject.TryGetComponent<Agent>(out var target);
            target.GetModule<HealthModule>().TakeDamage(_damage);
            Metrics metrics = _agent.Metrics;
            metrics.UpdateDamageDone(_damage);
            //Helpers.SpawnParticles(transform.position, Color.blue);
            Instantiate(explosionParticles, transform.position, Quaternion.identity);
            DebugManager.Instance.Log(
                $"{Helpers.CleanName(gameObject.name)} dealt {_damage} damage to {Helpers.CleanName(collision.transform.root.name)}",
                _agent.gameObject,
                target.gameObject
            );
            Destroy(gameObject);
        }
        else
        {
            Instantiate(explosionParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
