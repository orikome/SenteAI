using UnityEngine;

namespace SenteAI.Core
{
    public static class UtilityFunctions
    {
        public static float Calculate(
            float value,
            float optimal,
            float maxValue,
            UtilityType type
        ) =>
            type switch
            {
                UtilityType.Linear => Linear(value, maxValue),
                UtilityType.Quadratic => Quadratic(value, optimal),
                UtilityType.Gaussian => Gaussian(value, optimal, maxValue / 4),
                UtilityType.Exponential => Exponential(value, maxValue),
                _ => Linear(value, maxValue),
            };

        public static float Linear(float value, float max) => 1f - Mathf.Clamp01(value / max);

        public static float Quadratic(float value, float optimal) =>
            1f - Mathf.Pow((value - optimal) / optimal, 2);

        public static float Gaussian(float value, float optimal, float spread) =>
            Mathf.Exp(-(value - optimal) * (value - optimal) / (2 * spread * spread));

        public static float Exponential(float value, float max) => Mathf.Exp(-value / max);
    }
}
