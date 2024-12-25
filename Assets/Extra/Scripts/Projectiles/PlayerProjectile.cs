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

        Vector3 normal = collision.contacts[0].normal;
        Debug.DrawRay(collision.contacts[0].point, normal, Color.red, 2f);
        Quaternion hitRotation = Quaternion.FromToRotation(Vector3.forward, normal);

        if (OrikomeUtils.LayerMaskUtils.IsLayerInMask(collision.gameObject.layer, _collisionMask))
        {
            collision.transform.gameObject.TryGetComponent<Agent>(out var target);
            target.GetModule<HealthModule>().TakeDamage(_damage);
            Metrics metrics = _agent.Metrics;
            metrics.UpdateDamageDone(_damage);

            if (_agent.Faction == Faction.Player)
                CanvasManager.Instance.SpawnDamageText(
                    target.transform,
                    _damage.ToString(),
                    Color.white
                );

            Instantiate(explosionParticles, transform.position, hitRotation);
            DebugManager.Instance.Log(
                $"{Helpers.CleanName(gameObject.name)} dealt {_damage} damage to {Helpers.CleanName(collision.transform.root.name)}",
                _agent.gameObject,
                target.gameObject
            );
            Destroy(gameObject);
        }
        else
        {
            Instantiate(explosionParticles, transform.position, hitRotation);
            Destroy(gameObject);
        }
    }
}
