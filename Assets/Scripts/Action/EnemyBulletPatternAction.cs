using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/EnemyBulletPatternAction")]
public class EnemyBulletPatternAction : BulletPatternAction
{
    private Enemy _enemy;

    public override void Initialize(Agent agent)
    {
        base.Initialize(agent);
        _enemy = (Enemy)agent;
        heightOffset = -4f;
    }

    public override void CalculateUtility(Enemy agent)
    {
        float distance = agent.Metrics.DistanceToPlayer;
        float maxDistance = 100f;
        float CanSenseFactor = agent.GetModule<SenseModule>().CanSenseTarget ? 0.8f : 0.8f;
        float distanceFactor = 1.0f - (distance / maxDistance);
        float calculatedUtil = distanceFactor * 0.5f * CanSenseFactor;

        SetUtilityWithModifiers(calculatedUtil);
    }
}
