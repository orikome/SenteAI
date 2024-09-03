using UnityEngine;

public class AgentProjectile : Projectile
{
    protected override void Start()
    {
        base.Start();
        _collisionMask = LayerMask.GetMask("Player");
        SetColor(Color.red);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        int otherLayer = collision.gameObject.layer;
        if (!collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            OnMissCallback?.Invoke();
            Helpers.SpawnParticles(transform.position, Color.red);
            Destroy(gameObject);
            return;
        }

        damageable.TakeDamage(_damage);
        OnHitCallback?.Invoke();
        Helpers.SpawnParticles(transform.position, Color.red);
        Debug.Log(
            $"{Helpers.CleanName(gameObject.name)} dealt {_damage} damage to {Helpers.CleanName(collision.gameObject.name)}"
        );
        Destroy(gameObject);
    }
}
