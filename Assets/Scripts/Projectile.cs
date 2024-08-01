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

    private void Start()
    {
        timer = lifetime;
    }

    private void Update()
    {
        transform.Translate(moveDirection * 5f * Time.deltaTime);
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            OnMissCallback?.Invoke();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
            OnHitCallback?.Invoke();
            Destroy(gameObject);
            //Debug.Log($"{gameObject.name} dealt {damage} damage to {collision.gameObject.name}");
        }
        else
        {
            OnMissCallback?.Invoke();
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction;
    }
}
