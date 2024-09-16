using System;
using UnityEngine;

public class HomingOrbBehaviour : MonoBehaviour
{
    public float speed = 5f;
    public float homingIntensity = 0.5f;
    private Transform target;
    private Rigidbody rb;
    public LayerMask _collisionMask;
    private bool hasCompleted = false;
    public Action OnHitCallback;
    public Action OnMissCallback;
    private bool _isPlayer;

    public void SetParameters(bool isPlayer)
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 12f);
        _isPlayer = isPlayer;

        if (!isPlayer)
        {
            target = Player.Instance.transform;
            _collisionMask = LayerMask.GetMask("Enemy");
            gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
        }
        else
        {
            target = Player.Instance.Metrics.FindClosestEnemyToPlayer();
            _collisionMask = LayerMask.GetMask("Player");
            gameObject.layer = LayerMask.NameToLayer("PlayerProjectile");
        }
    }

    void Update()
    {
        if (hasCompleted || target == null)
            return;

        HomeTowardsTarget();
    }

    void OnDestroy()
    {
        if (!hasCompleted)
        {
            OnMissCallback?.Invoke();
            Destroy(gameObject);
        }
    }

    void HomeTowardsTarget()
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        rb.velocity = Vector3.Lerp(rb.velocity, directionToTarget * speed, homingIntensity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasCompleted)
            return;

        // Check if the collided object's layer is in the target collision mask
        if (OrikomeUtils.LayerMaskUtils.IsLayerInMask(collision.gameObject.layer, _collisionMask))
        {
            if (collision.transform.root.gameObject.TryGetComponent(out Agent agent))
            {
                agent.GetModule<HealthModule>().TakeDamage(10);
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
            // Handle cases where the collision does not match the targeted object (miss)
            Helpers.SpawnParticles(transform.position, Color.blue);
            hasCompleted = true;
            OnMissCallback?.Invoke();
            Destroy(gameObject);
        }
    }
}
