using System;
using UnityEngine;

public class LaserBehavior : MonoBehaviour
{
    private Enemy _agent;
    private float _damagePerSecond;
    public Action OnHitCallback;
    public Action OnMissCallback;
    private bool hasHitPlayer = false;

    public void Initialize(Enemy agent, float damagePerSecond)
    {
        _agent = agent;
        _damagePerSecond = damagePerSecond;
    }

    private void OnTriggerStay(Collider other)
    {
        if (
            OrikomeUtils.LayerMaskUtils.IsLayerInMask(
                other.gameObject.layer,
                LayerMask.GetMask("Player")
            )
        )
        {
            float damage = _damagePerSecond * Time.deltaTime;
            Player.Instance.GetModule<HealthModule>().TakeDamage((int)damage);

            // Invoke success callback once if hit
            if (!hasHitPlayer)
            {
                hasHitPlayer = true;
                OnHitCallback?.Invoke();
            }
        }
    }

    private void OnDestroy()
    {
        // Trigger failure if player was not hit
        if (!hasHitPlayer)
        {
            OnMissCallback?.Invoke();
        }
    }
}
