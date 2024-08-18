using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private int damage = 10;
    public float lifetime = 5f;
    private float timer;
    public Action OnHitCallback;
    public Action OnMissCallback;
    private Vector2 moveDirection;
    private LayerMask enemyProjectileMask;

    private void Start()
    {
        timer = lifetime;
        enemyProjectileMask = OrikomeUtils.LayerMaskUtils.CreateMask("EnemyProjectile", "Enemy");
    }

    private void Update()
    {
        transform.Translate(moveDirection * 5f * Time.deltaTime);
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            OnMissCallback?.Invoke();
            Helpers.SpawnParticles(transform.position, Color.white);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        int otherLayer = collision.gameObject.layer;
        if (OrikomeUtils.LayerMaskUtils.IsLayerInMask(otherLayer, enemyProjectileMask))
            return;

        if (!collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            OnMissCallback?.Invoke();
            Helpers.SpawnParticles(transform.position, Color.white);
            Destroy(gameObject);
            return;
        }

        damageable.TakeDamage(damage);
        OnHitCallback?.Invoke();
        Helpers.SpawnParticles(transform.position, Color.red);
        Destroy(gameObject);
        Debug.Log($"{gameObject.name} dealt {damage} damage to {collision.gameObject.name}");
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction;
    }
}
