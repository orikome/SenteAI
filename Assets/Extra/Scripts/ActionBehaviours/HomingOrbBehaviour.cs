using UnityEngine;

public class HomingOrbBehaviour : ActionBehaviour
{
    [SerializeField]
    private float rotationSpeed = 5f;

    [SerializeField]
    private float moveSpeed = 15f;

    [SerializeField]
    private float acceleration = 5f;
    private float currentSpeed;

    [SerializeField]
    private GameObject homingExplosionParticles;
    private Vector3 lastKnownDirection;

    public override void Initialize(Agent agent)
    {
        base.Initialize(agent);
        Destroy(gameObject, 12f);
        currentSpeed = moveSpeed * 0.5f;
    }

    private void Update()
    {
        if (hasHitTarget)
            return;

        if (_agent.Target == null)
        {
            ContinueLastKnownDirection();
        }
        else
        {
            HomeTowardsTarget();
        }
    }

    void OnDestroy()
    {
        if (!Application.isPlaying || (!gameObject.scene.isLoaded))
            return;

        if (!hasHitTarget)
        {
            Instantiate(homingExplosionParticles, transform.position, Quaternion.identity);
            OnMissCallback?.Invoke();
        }
    }

    void ContinueLastKnownDirection()
    {
        // Rotate towards last known direction
        Quaternion targetRotation = Quaternion.LookRotation(lastKnownDirection);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        // Keep accelerating
        currentSpeed = Mathf.Min(currentSpeed + acceleration * 4 * Time.deltaTime, moveSpeed);

        // Move forward using rotation
        transform.position += transform.forward * (currentSpeed * Time.deltaTime);
    }

    void HomeTowardsTarget()
    {
        Vector3 direction = (_agent.Target.transform.position - transform.position).normalized;
        lastKnownDirection = direction;
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

    private string GetLayerMaskNames(LayerMask mask)
    {
        var names = new System.Collections.Generic.List<string>();
        for (int i = 0; i < 32; i++)
        {
            if ((mask.value & (1 << i)) != 0)
            {
                string layerName = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(layerName))
                    names.Add(layerName);
            }
        }
        return string.Join(", ", names);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHitTarget)
            return;

        //     string debugInfo =
        //         $@"=== Collision Debug ===
        // Collider: {collision.collider.name}
        // Root: {collision.transform.root.name}
        // Layer: {LayerMask.LayerToName(collision.gameObject.layer)}
        // Target Mask Layers: {GetLayerMaskNames(_targetMask)}
        // Is In Mask: {(_targetMask.value & (1 << collision.gameObject.layer)) != 0}";

        //     Debug.Log(debugInfo);

        // if (
        //     !OrikomeUtils.LayerMaskUtils.IsLayerInMask(
        //         collision.gameObject.layer,
        //         Helpers.GetObstacleMask()
        //     )
        // )
        // {
        //     //Collision ignored - wrong layer
        //     return;
        // }

        Vector3 normal = collision.contacts[0].normal;
        //Debug.DrawRay(collision.contacts[0].point, normal, Color.red, 2f);
        Quaternion hitRotation = Quaternion.FromToRotation(Vector3.forward, normal);

        if (OrikomeUtils.LayerMaskUtils.IsLayerInMask(collision.gameObject.layer, _targetMask))
        {
            // Check if the collided object's layer is in the target collision mask
            if (collision.transform.TryGetComponent(out Agent agent))
            {
                agent.GetModule<HealthModule>().TakeDamage(40);
                _agent.Metrics.UpdateDamageDone(40);
                Instantiate(homingExplosionParticles, transform.position, hitRotation);
                AgentLogger.Log(
                    $"{Helpers.CleanName(gameObject.name)} dealt {40} damage to {Helpers.CleanName(collision.transform.root.name)}"
                );

                hasHitTarget = true;
                OnHitCallback?.Invoke();
                Destroy(gameObject);
            }
        }
        else
        {
            // Handle cases where the collision does not match the targeted object (miss)
            OnMissCallback?.Invoke();
        }

        // Spawn explosion effect and destroy projectile
        Instantiate(homingExplosionParticles, transform.position, hitRotation);
        hasHitTarget = true;
        Destroy(gameObject);
    }
}
