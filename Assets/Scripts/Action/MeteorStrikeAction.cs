using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/MeteorStrike")]
public class MeteorStrikeAction : AgentAction
{
    public GameObject meteorPrefab;
    public float dropDelay = 2f;
    private Enemy _enemy;

    public override void Initialize(Agent agent)
    {
        base.Initialize(agent);
        _enemy = (Enemy)agent;
    }

    public override void Execute(Transform firePoint, Vector3 direction)
    {
        DropMeteor(firePoint, _enemy);
        AfterExecution();
    }

    public override void CalculateUtility(Enemy agent)
    {
        float CanSenseFactor = agent.GetModule<SenseModule>().CanSenseTarget ? MIN_UTILITY : 1f;
        float calculatedUtil = 0.5f * CanSenseFactor;

        SetUtilityWithModifiers(calculatedUtil);
    }

    private void DropMeteor(Transform firePoint, Enemy agent)
    {
        PlayerMetrics playerMetrics = (PlayerMetrics)Player.Instance.Metrics;
        GameObject meteor = Instantiate(
            meteorPrefab,
            playerMetrics.PredictNextPositionUsingMomentum() + (Vector3.up * 10),
            Quaternion.identity
        );
        Destroy(meteor, dropDelay + 1f);
    }
}
