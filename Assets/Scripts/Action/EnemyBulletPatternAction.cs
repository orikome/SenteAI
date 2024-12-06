using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/EnemyBulletPatternAction")]
public class EnemyBulletPatternAction : BulletPatternAction
{
    private Agent _enemy;

    public override void Initialize(Agent agent)
    {
        base.Initialize(agent);
        _enemy = agent;
        heightOffset = -5.75f;
    }

    public override void CalculateUtility(Agent agent)
    {
        EnemyMetrics enemyMetrics = (EnemyMetrics)agent.Metrics;
        float distance = enemyMetrics.DistanceToTarget;
        float maxDistance = 100f;
        float CanSenseFactor = agent.GetModule<SenseModule>().CanSenseTarget ? 0.8f : 0.8f;
        float distanceFactor = 1.0f - (distance / maxDistance);
        float calculatedUtil = distanceFactor * 0.5f * CanSenseFactor;

        SetUtilityWithModifiers(calculatedUtil);
    }
}
