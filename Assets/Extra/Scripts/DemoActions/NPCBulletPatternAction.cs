using SenteAI.Core;
using UnityEngine;

namespace SenteAI.Extra
{
    [CreateAssetMenu(menuName = "SenteAI/Actions/NPCBulletPatternAction")]
    public class NPCBulletPatternAction : BulletPatternAction
    {
        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            heightOffset = -6.75f;
        }

        public override void CalculateUtility()
        {
            float utility = new UtilityBuilder()
                .WithDistance(_agent.Metrics.DistanceToTarget, 100f, UtilityType.Linear)
                .WithCustom(0.5f)
                .Build();

            SetUtilityWithModifiers(utility);
        }
    }
}
