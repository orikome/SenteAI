using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/MeteorStrike")]
public class MeteorStrikeAction : AgentAction
{
    public GameObject meteorPrefab;
    public float dropDelay = 2f;

    public override void Execute(Transform firePoint, EnemyAgent agent)
    {
        DropMeteor(firePoint, agent);
        AfterExecution();
    }

    public override void CalculateUtility(EnemyAgent agent)
    {
        float CanSenseFactor = agent.PerceptionModule.CanSenseTarget ? MIN_UTILITY : 1f;
        float calculatedUtil = 0.5f * CanSenseFactor;

        SetUtilityWithModifiers(calculatedUtil);
    }

    private void DropMeteor(Transform firePoint, EnemyAgent agent)
    {
        GameObject meteor = Instantiate(
            meteorPrefab,
            Player.Instance.Metrics.PredictNextPositionUsingMomentum() + (Vector3.up * 10),
            Quaternion.identity
        );
        Destroy(meteor, dropDelay + 1f);
    }
}
