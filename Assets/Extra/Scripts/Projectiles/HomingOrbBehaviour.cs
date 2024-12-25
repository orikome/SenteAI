using System;
using UnityEngine;

public class HomingOrbBehaviour : MonoBehaviour
{
    public float speed = 5f;
    public float homingIntensity = 0.5f;
    private Transform target;
    private Rigidbody rb;
    private LayerMask _targetMask;
    private LayerMask _ownerMask;
    private bool hasHitTarget = false;
    public Action OnHitCallback;
    public Action OnMissCallback;
    private bool _isPlayer;
    private Agent _enemy;

    [SerializeField]
    private float rotationSpeed = 5f;

    [SerializeField]
    private float moveSpeed = 15f;

    [SerializeField]
    private float acceleration = 5f;
    private float currentSpeed;

    public void SetParameters(Agent agent, bool isPlayer)
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 12f);
        _isPlayer = isPlayer;
        _enemy = agent;

        if (_isPlayer)
        {
            PlayerMetrics playerMetrics = (PlayerMetrics)AgentManager.Instance.playerAgent.Metrics;
            target = playerMetrics.FindClosestEnemyToPlayer();
            _targetMask = LayerMask.GetMask("Enemy");
            _ownerMask = LayerMask.GetMask("Player");
            gameObject.layer = LayerMask.NameToLayer("PlayerProjectile");
        }
        else
        {
            target = AgentManager.Instance.playerAgent.transform;
            _targetMask = LayerMask.GetMask("Player");
            _ownerMask = LayerMask.GetMask("Enemy");
            gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
        }
    }

    private void Start()
    {
        currentSpeed = moveSpeed * 0.25f;
    }

    private void Update()
    {
        if (hasHitTarget || target == null)
            return;

        HomeTowardsTarget();
    }

    void OnDestroy()
    {
        if (!hasHitTarget)
        {
            OnMissCallback?.Invoke();
        }
    }

    void HomeTowardsTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Smoothly rotate towards target
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        // Accelerate over time
        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, moveSpeed);

        // Move forward in the direction we're facing
        transform.position += transform.forward * (currentSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHitTarget)
            return;

        if (OrikomeUtils.LayerMaskUtils.IsLayerInMask(collision.gameObject.layer, _targetMask))
        {
            // Check if the collided object's layer is in the target collision mask
            if (collision.transform.TryGetComponent(out Agent agent))
            {
                agent.GetModule<HealthModule>().TakeDamage(10);
                _enemy.Metrics.UpdateDamageDone(10);
                Helpers.SpawnParticles(transform.position, Color.red);
                DebugManager.Instance.Log(
                    $"{Helpers.CleanName(gameObject.name)} dealt {10} damage to {Helpers.CleanName(collision.transform.root.name)}"
                );

                hasHitTarget = true;
                OnHitCallback?.Invoke();
                Destroy(gameObject);
            }
        }
        else
        {
            // Handle cases where the collision does not match the targeted object (miss)
            Helpers.SpawnParticles(transform.position, Color.blue);
            hasHitTarget = true;
            OnMissCallback?.Invoke();
            Destroy(gameObject);
        }
    }
}
