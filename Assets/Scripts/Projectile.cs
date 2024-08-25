using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    protected int damage = 10;
    public float lifetime = 5f;
    protected float timer;
    public Action OnHitCallback;
    public Action OnMissCallback;
    protected Vector3 moveDirection;
    protected LayerMask collisionMask;
    protected float speed;
    protected Vector3 rotationDirection;

    protected virtual void Start()
    {
        timer = lifetime;
    }

    protected virtual void FixedUpdate()
    {
        transform.Translate(speed * Time.fixedDeltaTime * moveDirection, Space.World);
        transform.Rotate(speed * Time.fixedDeltaTime * rotationDirection, Space.World);
    }

    protected virtual void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            OnMissCallback?.Invoke();
            Destroy(gameObject);
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        int otherLayer = collision.gameObject.layer;
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
        Debug.Log(
            $"{Helpers.CleanName(gameObject.name)} dealt {damage} damage to {Helpers.CleanName(collision.gameObject.name)}"
        );
        Destroy(gameObject);
    }

    public void Initialize(Vector3 direction, float projectileSpeed, int dmg)
    {
        moveDirection = direction.normalized;
        rotationDirection = direction.normalized;
        speed = projectileSpeed;
        damage = dmg;
    }
}
