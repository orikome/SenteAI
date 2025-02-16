using SenteAI.Core;
using UnityEngine;

namespace SenteAI.Extra
{
    public class PlayerProjectile : Projectile
    {
        private bool hasCompleted = false;

        protected override void OnCollisionEnter(Collision collision)
        {
            if (!_agent || hasCompleted)
                return;

            hasCompleted = true;

            Vector3 normal = collision.contacts[0].normal;
            //Debug.DrawRay(collision.contacts[0].point, normal, Color.red, 2f);
            Quaternion hitRotation = Quaternion.FromToRotation(Vector3.forward, normal);

            if (OrikomeUtils.LayerMaskUtils.IsLayerInMask(collision.gameObject.layer, _targetMask))
            {
                collision.transform.gameObject.TryGetComponent<Agent>(out var target);
                target.GetModule<HealthModule>().TakeDamage(Damage, transform.forward);
                Metrics metrics = _agent.Metrics;
                metrics.UpdateDamageDone(Damage);

                if (_agent.Faction == Faction.Player)
                    CanvasManager.Instance.SpawnDamageText(
                        target.transform,
                        Damage.ToString(),
                        Color.white
                    );

                Instantiate(explosionParticles, transform.position, hitRotation);
                AgentLogger.Log(
                    $"{Helpers.CleanName(gameObject.name)} dealt {Damage} damage to {Helpers.CleanName(collision.transform.root.name)}",
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
}
