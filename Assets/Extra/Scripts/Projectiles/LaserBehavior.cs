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

    public void Initialize(Agent agent, float damagePerSecond)
    {
        _agent = agent;
        _damagePerSecond = damagePerSecond;

        if (_agent == null || agent.Target == null)
            return;

        _targetMask = Helpers.GetTargetMask(_agent.Faction);
        _ownerMask = Helpers.GetOwnerMask(_agent.Faction);
        gameObject.layer = Helpers.GetProjectileLayer(_agent.Faction);
    }

    private void OnTriggerStay(Collider other)
    {
        if (OrikomeUtils.LayerMaskUtils.IsLayerInMask(other.gameObject.layer, _targetMask))
        {
            float damage = _damagePerSecond * Time.deltaTime;

            if (other.transform.TryGetComponent(out Agent targetAgent))
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
