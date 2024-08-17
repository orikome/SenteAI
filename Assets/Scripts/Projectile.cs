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
            TriggerParticles();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
            OnHitCallback?.Invoke();
            TriggerParticles();
            Destroy(gameObject);
            //Debug.Log($"{gameObject.name} dealt {damage} damage to {collision.gameObject.name}");
        }
        else
        {
            OnMissCallback?.Invoke();
            TriggerParticles();
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction;
    }

    private void TriggerParticles()
    {
        GameObject particles = new GameObject("BulletParticles");
        ParticleSystem particleSystem = particles.AddComponent<ParticleSystem>();
        particles.transform.position = transform.position;

        var main = particleSystem.main;
        main.startLifetime = 0.2f;
        main.startSpeed = 30f;
        main.startSize = 0.4f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startColor = Color.white;

        var renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        renderer.material.SetColor("_TintColor", Color.white);

        // Fade out smoothly
        var colorOverLifetime = particleSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(Color.white, 0.0f),
                new GradientColorKey(Color.white, 1.0f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        var emission = particleSystem.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 15) });

        var shape = particleSystem.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.2f;

        var velocityOverLifetime = particleSystem.velocityOverLifetime;
        velocityOverLifetime.enabled = true;
        velocityOverLifetime.space = ParticleSystemSimulationSpace.Local;
        velocityOverLifetime.radial = 10f;

        var noise = particleSystem.noise;
        noise.enabled = true;
        noise.strength = 0.7f;
        noise.frequency = 3f;
        noise.damping = true;

        var trails = particleSystem.trails;
        trails.enabled = true;
        trails.lifetime = 0.2f;
        trails.dieWithParticles = true;
        trails.widthOverTrail = 0.2f;
        trails.colorOverLifetime = new ParticleSystem.MinMaxGradient(Color.white);

        var trailMaterial = new Material(Shader.Find("Particles/Standard Unlit"));
        trailMaterial.SetColor("_TintColor", Color.white);
        renderer.trailMaterial = trailMaterial;

        Destroy(particles, main.startLifetime.constant + 0.5f);
    }
}
