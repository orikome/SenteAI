using System;
using UnityEngine;

public class HomingOrbBehaviour : MonoBehaviour
{
    public float speed = 5f;
    public float homingIntensity = 0.5f;
    private Transform player;
    private Rigidbody rb;
    public LayerMask _collisionMask;
    private bool hasCompleted = false;
    public Action OnHitCallback;
    public Action OnMissCallback;

    void Start()
    {
        player = Player.Instance.transform;
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 12f);
        _collisionMask = LayerMask.GetMask("Player");
    }

    void Update()
    {
        if (hasCompleted || player == null)
            return;

        HomeTowardsPlayer();
    }

    void OnDestroy()
    {
        if (!hasCompleted)
        {
            OnMissCallback?.Invoke();
            Destroy(gameObject);
        }
    }

    void HomeTowardsPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        rb.velocity = Vector3.Lerp(rb.velocity, directionToPlayer * speed, homingIntensity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasCompleted)
            return;

        if (OrikomeUtils.LayerMaskUtils.IsLayerInMask(collision.gameObject.layer, _collisionMask))
        {
            if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(10);
                Helpers.SpawnParticles(transform.position, Color.red);
                DebugManager.Instance.Log(
                    $"{Helpers.CleanName(gameObject.name)} dealt {10} damage to {Helpers.CleanName(collision.transform.root.name)}"
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
