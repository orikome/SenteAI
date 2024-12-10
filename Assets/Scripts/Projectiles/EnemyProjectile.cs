using UnityEngine;

public class EnemyProjectile : Projectile
{
    private bool hasCompleted = false;
    private bool hasPassedTarget = false;

    protected override void Start()
    {
        base.Start();
        _collisionMask = LayerMask.GetMask("Player", "Ally");
    }

    protected override void Update()
    {
        _timer -= Time.deltaTime;

        if (hasCompleted)
            return;

        // Check if projectile has passed the target, but don't destroy it yet
        if (!hasPassedTarget && HasPassedTarget())
        {
            hasPassedTarget = true;
            OnMissCallback?.Invoke();
        }

        // Destroy projectile when the timer runs out
        if (_timer <= 0f)
        {
            hasCompleted = true;
            Destroy(gameObject);
        }
    }

    private bool HasPassedTarget()
    {
        Vector3 toTarget = _agent.Target.transform.position - transform.position;
        // If the dot product is negative, projectile is facing away from the target
        return Vector3.Dot(transform.forward, toTarget) < 0;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (hasCompleted)
            return;

        if (OrikomeUtils.LayerMaskUtils.IsLayerInMask(collision.gameObject.layer, _collisionMask))
        {
            if (collision.transform.root.gameObject.TryGetComponent<Agent>(out var target))
            {
                target.GetModule<HealthModule>().TakeDamage(10);
                Helpers.SpawnParticles(transform.position, Color.red);
                _agent.Metrics.UpdateDamageDone(10);
                DebugManager.Instance.Log(
                    $"{Helpers.CleanName(gameObject.name)} dealt {_damage} damage to {Helpers.CleanName(collision.transform.root.name)}"
                );
                OnHitCallback?.Invoke();
                hasCompleted = true;
                Destroy(gameObject);
            }
        }
        else
        {
            Helpers.SpawnParticles(transform.position, Color.blue);
            hasCompleted = true;
            OnMissCallback?.Invoke();
            Destroy(gameObject);
        }
    }
}
