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
        float utility = new UtilityBuilder()
            .WithDistance(agent.Metrics.DistanceToTarget, 100f, UtilityType.Linear)
            .WithCustom(0.5f)
            .Build();

        SetUtilityWithModifiers(utility);
    }
}
