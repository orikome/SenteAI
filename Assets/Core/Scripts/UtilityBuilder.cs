using UnityEngine;

public class UtilityBuilder
{
    private float _utility = 1.0f;

    public UtilityBuilder WithDistance(
        float distance,
        float maxDistance,
        UtilityType type = UtilityType.Linear
    )
    {
        _utility *= UtilityFunctions.Calculate(distance, maxDistance / 2f, maxDistance, type);
        return this;
    }

    public UtilityBuilder WithSensing(bool canSense, float weight = 0.8f)
    {
        _utility *= canSense ? weight : Utility.MIN_UTILITY;
        return this;
    }

    public UtilityBuilder WithInverseSensing(bool canSense, float weight = 0.8f)
    {
        _utility *= !canSense ? weight : 0.6f;
        return this;
    }

    public UtilityBuilder WithHealth(
        float current,
        float max,
        UtilityType type = UtilityType.Linear
    )
    {
        _utility *= UtilityFunctions.Calculate(current, max / 2f, max, type);
        return this;
    }

    public UtilityBuilder WithCustom(float factor)
    {
        _utility *= Mathf.Clamp01(factor);
        return this;
    }

    public UtilityBuilder WithMeleeDistance(float distance, float optimalRange, float maxRange)
    {
        if (distance <= optimalRange)
            _utility *= 1f;
        else
            _utility *= Mathf.Lerp(1f, 0f, (distance - optimalRange) / (maxRange - optimalRange));
        return this;
    }

    public UtilityBuilder WithAggression(float currentHealth, float maxHealth)
    {
        float healthPercent = currentHealth / maxHealth;
        _utility *= Mathf.Lerp(1.5f, 1f, healthPercent);
        return this;
    }

    public UtilityBuilder WithProjectileStats(float projectileSpeed, float maxSpeed = 30f)
    {
        _utility *= Mathf.Clamp01(projectileSpeed / maxSpeed);
        return this;
    }

    public float Build() => Mathf.Clamp(_utility, Utility.MIN_UTILITY, Utility.MAX_UTILITY);
}
