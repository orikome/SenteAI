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
        if (OrikomeUtils.LayerMaskUtils.IsLayerInMask(collision.gameObject.layer, _collisionMask))
        {
            collision.transform.root.gameObject.TryGetComponent<Agent>(out var agent);
            agent.GetModule<HealthModule>().TakeDamage(10);
            PlayerMetrics playerMetrics = (PlayerMetrics)Player.Instance.Metrics;
            playerMetrics.UpdateDamageDone(10);
            Helpers.SpawnParticles(transform.position, Color.blue);
            DebugManager.Instance.Log(
                $"{Helpers.CleanName(gameObject.name)} dealt {_damage} damage to {Helpers.CleanName(collision.transform.root.name)}"
            );
            Destroy(gameObject);
        }
        else
        {
            Helpers.SpawnParticles(transform.position, Color.blue);
            Destroy(gameObject);
        }
    }
}
