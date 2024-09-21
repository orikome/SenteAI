using UnityEngine;

public class AgentProjectile : Projectile
{
    private bool hasCompleted = false;
    private bool hasPassedPlayer = false;

    protected override void Start()
    {
        base.Start();
        _collisionMask = LayerMask.GetMask("Player");
    }

    protected override void Update()
    {
        _timer -= Time.deltaTime;

        if (hasCompleted)
            return;

        // Check if projectile has passed player, but don't destroy it yet
        if (!hasPassedPlayer && HasPassedPlayer())
        {
            hasPassedPlayer = true;
            OnMissCallback?.Invoke();
        }

        // Destroy projectile when the timer runs out
        if (_timer <= 0f)
        {
            hasCompleted = true;
            Destroy(gameObject);
        }
    }

    private bool HasPassedPlayer()
    {
        Vector3 toPlayer = GameManager.Instance.playerAgent.transform.position - transform.position;
        // If dot product is negative, projectile is facing away from the player
        return Vector3.Dot(transform.forward, toPlayer) < 0;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (hasCompleted)
            return;

        if (OrikomeUtils.LayerMaskUtils.IsLayerInMask(collision.gameObject.layer, _collisionMask))
        {
            if (collision.transform.root.gameObject.TryGetComponent<Agent>(out var player))
            {
                player.GetModule<HealthModule>().TakeDamage(10);
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
