using UnityEngine;

[CreateAssetMenu(menuName = "SenteAI/Actions/MeteorStrike")]
public class MeteorStrikeAction : AgentAction
{
    public GameObject meteorPrefab;
    public float dropDelay = 2f;
    public GameObject warningIndicator;
    private float clearanceCheckRadius = 20f;

    public override void Execute(Transform firePoint, Vector3 direction)
    {
        if (!IsLandingAreaClear(_agent.Target.Metrics.PredictPosition()))
        {
            AddCooldown();
            return;
        }

        DropMeteor();
        AfterExecution();
    }

    public override void CalculateUtility(Agent agent)
    {
        //float CanSenseFactor = agent.GetModule<SenseModule>().CanSenseTarget ? MIN_UTILITY : 1f;
        //float calculatedUtil = 2.5f * CanSenseFactor;

        SetUtilityWithModifiers(2.5f);
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
        Metrics targetMetrics = _agent.Target.Metrics;

        if (targetMetrics == null)
            return;
        // Spawn warning indicator
        GameObject obj = Instantiate(
            warningIndicator,
            targetMetrics.PredictPosition(),
            Quaternion.identity
        );

        obj.GetComponentInChildren<WarningIndicator>().Initialize(_agent);

        GameObject meteor = Instantiate(
            meteorPrefab,
            targetMetrics.PredictPosition() + (Vector3.up * 60),
            Quaternion.identity
        );
        meteor.GetComponent<MeteorBehavior>().Initialize(_agent);
        Destroy(meteor, dropDelay + 1f);
    }
}
