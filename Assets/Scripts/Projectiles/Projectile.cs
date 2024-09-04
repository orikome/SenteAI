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
    private TrailRenderer[] trailRenderers;
    public Gradient speedGradient;

    protected virtual void Start() { }

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

    public void Initialize()
    {
        _timer = lifetime;
        _renderer = GetComponent<Renderer>();
        _light = GetComponentInChildren<Light>();
        trailRenderers = GetComponentsInChildren<TrailRenderer>();
    }

    protected void SetColor(Color passedColor, float passedOpacity = 1.0f)
    {
        _renderer.material.SetColor("_Color", passedColor);
        if (_renderer.material.HasProperty("_EmissionColor"))
        {
            _renderer.material.SetColor("_EmissionColor", passedColor);
            _renderer.material.EnableKeyword("_EMISSION");
        }

        foreach (var trail in trailRenderers)
        {
            trail.startColor = passedColor;
            trail.endColor = passedColor;
        }
        _light.color = passedColor;
    }

    private Color GetColorBySpeed(float speed)
    {
        float minSpeed = 0f;
        float maxSpeed = 30f;

        float clampedSpeed = Mathf.Clamp(speed, minSpeed, maxSpeed);

        // Value between 0 and 1
        float normalizedSpeed = (clampedSpeed - minSpeed) / (maxSpeed - minSpeed);
        return speedGradient.Evaluate(normalizedSpeed);
    }

    public void SetParameters(Vector3 direction, float projectileSpeed, int dmg)
    {
        Initialize();
        _moveDirection = direction.normalized;
        _rotationDirection = direction.normalized;
        _speed = projectileSpeed;
        _damage = dmg;
        _timer = lifetime;
        SetColor(GetColorBySpeed(projectileSpeed));
    }
}
