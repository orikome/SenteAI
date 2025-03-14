using SenteAI.Core;
using UnityEngine;

namespace SenteAI.Extra
{
    [CreateAssetMenu(menuName = "SenteAI/Actions/NPCMeteorStrike")]
    public class NPCMeteorStrikeAction : AgentAction
    {
        public GameObject meteorPrefab;
        public float dropDelay = 2f;
        public GameObject warningIndicator;
        private float clearanceCheckRadius = 20f;

        public override void Execute(Transform firePoint, Vector3 direction)
        {
            if (!IsLandingAreaClear(_agent.Target.Metrics.GetPredictedPosition()))
            {
                AddCooldown();
                return;
            }

            DropMeteor();
            AfterExecution();
        }

        public override void CalculateUtility()
        {
            float utility = new UtilityBuilder()
                .WithDistance(_agent.Metrics.DistanceToTarget, 100f, UtilityType.Linear)
                .WithLOS(_agent.GetModule<SeeingModule>().HasLOS)
                .Build();

            SetUtilityWithModifiers(utility);
        }

        private bool IsLandingAreaClear(Vector3 position)
        {
            Collider[] colliders = Physics.OverlapSphere(
                position,
                clearanceCheckRadius,
                Helpers.GetObstacleMask()
            );
            return colliders.Length == 0;
        }

        private void DropMeteor()
        {
            Vector3 targetPos = _agent.Target.transform.position;

            Vector3 spawnPosition = targetPos;
            spawnPosition.y = 0.001f;
            // Spawn warning indicator
            GameObject obj = Instantiate(warningIndicator, spawnPosition, Quaternion.identity);

            obj.GetComponentInChildren<WarningIndicator>().Initialize(_agent);

            GameObject meteor = Instantiate(
                meteorPrefab,
                targetPos + (Vector3.up * 60),
                Quaternion.identity
            );
            meteor.GetComponent<MeteorBehavior>().Initialize(_agent);
            Destroy(meteor, dropDelay + 1f);
        }
    }
}
