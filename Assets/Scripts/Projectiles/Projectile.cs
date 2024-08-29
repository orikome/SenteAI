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
    public Renderer _renderer;
    private Light _light;

    protected virtual void Start()
    {
        _timer = lifetime;
        _renderer = GetComponent<Renderer>();
        _light = GetComponentInChildren<Light>();
        _renderer.material.SetColor("_Color", Color.blue);
        if (_renderer.material.HasProperty("_EmissionColor"))
        {
            _renderer.material.SetColor("_EmissionColor", Color.blue);
            _renderer.material.EnableKeyword("_EMISSION");
        }

        var trailRenderers = GetComponentsInChildren<TrailRenderer>();
        foreach (var trail in trailRenderers)
        {
            trail.startColor = Color.blue;
            trail.endColor = Color.blue;
        }
        _light.color = Color.blue;
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

    protected virtual void OnCollisionEnter(Collision collision) { }

    public void Initialize() { }

    public void SetParameters(Vector3 direction, float projectileSpeed, int dmg)
    {
        _moveDirection = direction.normalized;
        _rotationDirection = direction.normalized;
        _speed = projectileSpeed;
        _damage = dmg;
        _timer = lifetime;
    }
}
