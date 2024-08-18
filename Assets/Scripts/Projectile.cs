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
    private Vector3 moveDirection;
    private LayerMask enemyProjectileMask;
    private float speed;
    Vector3 rotationDirection;

    private void Start()
    {
        timer = lifetime;
        enemyProjectileMask = OrikomeUtils.LayerMaskUtils.CreateMask("EnemyProjectile", "Enemy");
    }

    private void FixedUpdate()
    {
        transform.Translate(speed * Time.fixedDeltaTime * moveDirection, Space.World);
        transform.Rotate(speed * Time.fixedDeltaTime * rotationDirection, Space.World);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            OnMissCallback?.Invoke();
            //Helpers.SpawnParticles(transform.position, Color.white);
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

    public void Initialize(Vector3 direction, float projectileSpeed, int dmg)
    {
        moveDirection = direction.normalized;
        rotationDirection = direction.normalized;
        speed = projectileSpeed;
        damage = dmg;
    }
}
