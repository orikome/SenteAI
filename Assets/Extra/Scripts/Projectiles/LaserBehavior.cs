using System;
using UnityEngine;

public class LaserBehavior : MonoBehaviour
{
    private Agent _agent;
    private float _damagePerSecond;
    public Action OnHitCallback;
    public Action OnMissCallback;
    private bool hasHitTarget = false;
    private bool _isPlayer;

    private LayerMask _targetMask;
    private LayerMask _ownerMask;

    public void Initialize(Agent agent, float damagePerSecond, bool isPlayer)
    {
        _agent = agent;
        _damagePerSecond = damagePerSecond;
        _isPlayer = isPlayer;

        if (_isPlayer)
        {
            _targetMask = LayerMask.GetMask("Enemy");
            _ownerMask = LayerMask.GetMask("Player");
            gameObject.layer = LayerMask.NameToLayer("PlayerProjectile");
        }
        else
        {
            _targetMask = LayerMask.GetMask("Player");
            _ownerMask = LayerMask.GetMask("Enemy");
            gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (OrikomeUtils.LayerMaskUtils.IsLayerInMask(other.gameObject.layer, _targetMask))
        {
            float damage = _damagePerSecond * Time.deltaTime;

            if (other.transform.root.TryGetComponent(out Agent targetAgent))
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
