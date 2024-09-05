using UnityEngine;

public static class Helpers
{
    public static Vector3 PredictPosition(
        Vector3 shooterPosition,
        Transform target,
        float projectileSpeed,
        float accuracy = 1.0f,
        Vector3 lastTargetPosition = default
    )
    {
        Vector3 currentTargetPosition = target.position;
        Vector3 targetVelocity = (currentTargetPosition - lastTargetPosition) / Time.deltaTime;

        // Get distance between shooter and target
        float distance = OrikomeUtils.GeneralUtils.GetDistanceSquared(
            target.position,
            shooterPosition
        );

        float timeToTarget = distance / projectileSpeed;

        Vector3 predictedPosition = target.position + targetVelocity * timeToTarget;

        // If accuracy is low, add deviation to make the prediction imperfect
        float deviationMagnitude = (1.0f - accuracy) * 0.5f;
        Vector3 deviation = Random.insideUnitSphere * deviationMagnitude;
        Vector3 adjustedPrediction = predictedPosition + deviation;

        return (adjustedPrediction - shooterPosition).normalized;
    }

    public static void SpawnParticles(
        Vector3 position,
        Color color,
        float lifetime = 0.2f,
        float speed = 30f,
        float size = 0.4f,
        int burstCount = 15
    )
    {
        GameObject particles = new("CustomParticles");
        ParticleSystem particleSystem = particles.AddComponent<ParticleSystem>();
        particles.transform.position = position;

        var main = particleSystem.main;
        main.startLifetime = lifetime;
        main.startSpeed = speed;
        main.startSize = size;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startColor = color;

        var renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        renderer.material.SetColor("_TintColor", color);

        // Fade out smoothly
        var colorOverLifetime = particleSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new();
        gradient.SetKeys(
            new GradientColorKey[] { new(color, 0.0f), new(color, 1.0f) },
            new GradientAlphaKey[] { new(1.0f, 0.0f), new(0.0f, 1.0f) }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        var emission = particleSystem.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, burstCount) });

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
        trails.colorOverLifetime = new ParticleSystem.MinMaxGradient(color);

        var trailMaterial = new Material(Shader.Find("Particles/Standard Unlit"));
        trailMaterial.SetColor("_TintColor", color);
        renderer.trailMaterial = trailMaterial;

        GameObject.Destroy(particles, main.startLifetime.constant + 0.5f);
    }

    public static string CleanName(string name)
    {
        return name.Replace("(Clone)", "").Trim();
    }

    public static void DebugLog(AgentAction actionToUse, Transform transform)
    {
        string actionName = CleanName(actionToUse.name);

        float utilityScore = actionToUse.utilityScore;

        DebugManager.Instance.Log(transform, $"{actionName}={utilityScore:F2}", Color.cyan);
    }
}
