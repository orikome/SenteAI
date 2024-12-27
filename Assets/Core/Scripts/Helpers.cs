using UnityEngine;

public static class Helpers
{
    private static readonly string PLAYER_COLOR = "#800080"; // Purple
    private static readonly string ENEMY_COLOR = "#FFB3B3"; // Pastel Red
    private static readonly string ALLY_COLOR = "#B3FFB3"; // Pastel Green

    public static string GetFactionColor(Faction faction)
    {
        return faction switch
        {
            Faction.Player => PLAYER_COLOR,
            Faction.Enemy => ENEMY_COLOR,
            Faction.Ally => ALLY_COLOR,
            _ => "#FFFFFF", // Default white
        };
    }

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

    public static LayerMask GetTargetMask(Faction faction)
    {
        return faction switch
        {
            Faction.Enemy => LayerMask.GetMask("Player", "Ally"),
            Faction.Player or Faction.Ally => LayerMask.GetMask("Enemy"),
            _ => LayerMask.GetMask("Default"),
        };
    }

    public static LayerMask GetOwnerMask(Faction faction)
    {
        return faction switch
        {
            Faction.Player => LayerMask.GetMask("Player"),
            Faction.Enemy => LayerMask.GetMask("Enemy"),
            Faction.Ally => LayerMask.GetMask("Ally"),
            _ => LayerMask.GetMask("Default"),
        };
    }

    public static int GetProjectileLayer(Faction faction)
    {
        return faction switch
        {
            Faction.Enemy => LayerMask.NameToLayer("EnemyProjectile"),
            Faction.Player or Faction.Ally => LayerMask.NameToLayer("PlayerProjectile"),
            _ => LayerMask.NameToLayer("Default"),
        };
    }

    public static int GetObstacleMask()
    {
        return LayerMask.GetMask("Wall");
    }

    public static void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
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
}
