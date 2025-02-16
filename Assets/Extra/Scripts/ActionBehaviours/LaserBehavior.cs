using SenteAI.Core;
using UnityEngine;

namespace SenteAI.Extra
{
    public class LaserBehavior : ActionBehaviour
    {
        private float _damagePerSecond = 100f;

        private void OnTriggerStay(Collider other)
        {
            if (OrikomeUtils.LayerMaskUtils.IsLayerInMask(other.gameObject.layer, _targetMask))
            {
                float damage = _damagePerSecond * Time.deltaTime;

                if (other.transform.gameObject.TryGetComponent(out Agent targetAgent))
                {
                    targetAgent.GetModule<HealthModule>().TakeDamage((int)damage);
                    _agent.Metrics.UpdateDamageDone(damage);

                    if (!hasHitTarget)
                    {
                        hasHitTarget = true;
                        OnHitCallback?.Invoke();
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (!Application.isPlaying || (!gameObject.scene.isLoaded))
                return;

            // Trigger failure if target was not hit
            if (!hasHitTarget)
            {
                OnMissCallback?.Invoke();
            }
        }
    }
}
