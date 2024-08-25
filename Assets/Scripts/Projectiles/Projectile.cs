using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Action OnHitCallback;
    public Action OnMissCallback;
    protected int _damage = 10;
    public float lifetime = 5f;
    protected float _timer;
    protected Vector3 _moveDirection;
    protected LayerMask _collisionMask;
    protected float _speed;
    protected Vector3 _rotationDirection;

    protected virtual void Start()
    {
        _timer = lifetime;
    }

    protected virtual void FixedUpdate()
    {
        transform.Translate(_speed * Time.fixedDeltaTime * _moveDirection, Space.World);
        transform.Rotate(_speed * Time.fixedDeltaTime * _rotationDirection, Space.World);
    }

    protected virtual void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
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

        damageable.TakeDamage(_damage);
        OnHitCallback?.Invoke();
        Helpers.SpawnParticles(transform.position, Color.red);
        Debug.Log(
            $"{Helpers.CleanName(gameObject.name)} dealt {_damage} damage to {Helpers.CleanName(collision.gameObject.name)}"
        );
        Destroy(gameObject);
    }

    public void Initialize(Vector3 direction, float projectileSpeed, int dmg)
    {
        _moveDirection = direction.normalized;
        _rotationDirection = direction.normalized;
        _speed = projectileSpeed;
        _damage = dmg;
    }
}
