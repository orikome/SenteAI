using UnityEngine;

[CreateAssetMenu(menuName = "SenteAI/Actions/MeteorStrike")]
public class MeteorStrikeAction : AgentAction
{
    public GameObject meteorPrefab;
    public float dropDelay = 2f;
    public GameObject warningIndicator;

    public override void Execute(Transform firePoint, Vector3 direction)
    {
        DropMeteor();
        AfterExecution();
    }

    public override void CalculateUtility(Agent agent)
    {
        float CanSenseFactor = agent.GetModule<SenseModule>().CanSenseTarget ? MIN_UTILITY : 1f;
        float calculatedUtil = 0.5f * CanSenseFactor;

        SetUtilityWithModifiers(calculatedUtil);
    }

    private void DropMeteor()
    {
        Metrics targetMetrics = _agent.Target.Metrics;

        if (targetMetrics == null)
            return;
        // Spawn warning indicator
        GameObject obj = Instantiate(
            warningIndicator,
            targetMetrics.PredictNextPositionUsingMomentum(),
            Quaternion.identity
        );

        obj.GetComponentInChildren<WarningIndicator>().Initialize(_agent);

        GameObject meteor = Instantiate(
            meteorPrefab,
            targetMetrics.PredictNextPositionUsingMomentum() + (Vector3.up * 60),
            Quaternion.identity
        );
        meteor.GetComponent<MeteorBehavior>().Initialize(_agent);
        Destroy(meteor, dropDelay + 1f);
    }
}
