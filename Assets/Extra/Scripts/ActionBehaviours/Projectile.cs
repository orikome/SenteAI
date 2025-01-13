using System;
using UnityEngine;

public class Projectile : ActionBehaviour
{
    public int Damage { get; protected set; } = 10;
    public float Speed { get; protected set; }
    public Vector3 MoveDirection { get; protected set; }
    public Vector3 RotationDirection { get; protected set; }
    public float lifetime = 5f;
    protected float _timer;
    public Renderer _renderer;
    private Light _light;
    private TrailRenderer[] trailRenderers;
    public Gradient speedGradient;

    [SerializeField]
    protected GameObject explosionParticles;
    protected const string SHADER_COLOR_PROPERTY = "_MainColor";
    protected const string SHADER_ORB_COLOR = "_OrbColor";
    protected int deflectedCount = 0;

    public virtual void SetParameters(
        Agent agent,
        Vector3 direction,
        float projectileSpeed,
        int dmg
    )
    {
        Initialize(agent);
        MoveDirection = direction.normalized;
        RotationDirection = direction.normalized;
        Speed = projectileSpeed;
        Damage = dmg;
        _timer = lifetime;
        //SetColor(GetColorBySpeed(projectileSpeed));

        //Debug.DrawRay(transform.position, direction.normalized * 4f, Color.blue, 1f);
    }

    public override void Initialize(Agent agent)
    {
        base.Initialize(agent);
        _timer = lifetime;
        _renderer = GetComponent<Renderer>();
        _light = GetComponentInChildren<Light>();
        trailRenderers = GetComponentsInChildren<TrailRenderer>();
    }

    protected virtual void FixedUpdate()
    {
        transform.Translate(Speed * Time.fixedDeltaTime * MoveDirection, Space.World);
        transform.Rotate(Speed * Time.fixedDeltaTime * RotationDirection, Space.World);
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

    public void SetColor(Color passedColor, float passedOpacity = 1.0f)
    {
        if (_renderer == null || _renderer.material == null)
            return;

        _renderer.material.SetColor("_Color", passedColor);
        _renderer.material.SetColor(SHADER_COLOR_PROPERTY, passedColor);
        _renderer.material.SetColor(SHADER_ORB_COLOR, passedColor);
        _renderer.material.EnableKeyword("_EMISSION");
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
}
