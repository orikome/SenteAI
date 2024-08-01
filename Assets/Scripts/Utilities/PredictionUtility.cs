using UnityEngine;

public static class PredictionUtility
{
    public static Vector3 PredictPosition(
        Vector3 shooterPosition,
        Transform target,
        float projectileSpeed,
        float accuracy = 1.0f
    )
    {
        if (!target.GetComponent<Rigidbody>())
            return (target.position - shooterPosition).normalized;

        Vector3 directionToTarget = (target.position - shooterPosition).normalized;
        Vector3 targetVelocity = target.GetComponent<Rigidbody>().velocity;

        float timeToTarget = Vector3.Distance(target.position, shooterPosition) / projectileSpeed;

        Vector3 predictedPosition = target.position + targetVelocity * timeToTarget;

        // The lower the accuracy, the higher the deviation
        float deviationMagnitude = (1.0f - accuracy) * 0.5f;
        Vector3 deviation = Random.insideUnitSphere * deviationMagnitude;
        Vector3 adjustedPrediction = predictedPosition + deviation;

        return (adjustedPrediction - shooterPosition).normalized;
    }
}
