using UnityEngine;

[CreateAssetMenu(menuName = "SenteAI/Actions/NPCBulletPatternAction")]
public class NPCBulletPatternAction : BulletPatternAction
{
    public override void Initialize(Agent agent)
    {
        base.Initialize(agent);
        heightOffset = -3.75f;
    }

    public override void CalculateUtility(Agent agent)
    {
        Metrics metrics = agent.Metrics;
        float distance = metrics.DistanceToTarget;
        float maxDistance = 100f;
        float CanSenseFactor = agent.GetModule<SenseModule>().CanSenseTarget ? 0.8f : 0.8f;
        float distanceFactor = 1.0f - (distance / maxDistance);
        float calculatedUtil = distanceFactor * 0.5f * CanSenseFactor;

        SetUtilityWithModifiers(calculatedUtil);
    }
}
