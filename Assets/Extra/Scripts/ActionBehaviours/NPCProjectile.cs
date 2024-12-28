using UnityEngine;

public class NPCProjectile : Projectile
{
    private bool hasCompleted = false;
    private bool hasPassedTarget = false;

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
        if (_agent.Target == null)
            return false;

        Vector3 toTarget = _agent.Target.transform.position - transform.position;
        // If the dot product is negative, projectile is facing away from the target
        return Vector3.Dot(transform.forward, toTarget) < 0;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (hasCompleted || !_agent)
            return;

        Vector3 normal = collision.contacts[0].normal;
        //Debug.DrawRay(collision.contacts[0].point, normal, Color.red, 2f);
        Quaternion hitRotation = Quaternion.FromToRotation(Vector3.forward, normal);

        if (OrikomeUtils.LayerMaskUtils.IsLayerInMask(collision.gameObject.layer, _targetMask))
        {
            if (collision.transform.gameObject.TryGetComponent<Agent>(out var target))
            {
                target.GetModule<HealthModule>().TakeDamage(_damage);
                Instantiate(explosionParticles, transform.position, hitRotation);
                _agent.Metrics.UpdateDamageDone(_damage);
                AgentLogger.Log(
                    $"{Helpers.CleanName(gameObject.name)} dealt {_damage} damage to {Helpers.CleanName(collision.transform.name)}",
                    _agent.gameObject,
                    target.gameObject
                );
                OnHitCallback?.Invoke();
                hasCompleted = true;
                Destroy(gameObject);
            }
        }
        else
        {
            Instantiate(explosionParticles, transform.position, hitRotation);
            hasCompleted = true;
            OnMissCallback?.Invoke();
            Destroy(gameObject);
        }
    }
}
