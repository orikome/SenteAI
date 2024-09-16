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

    public override void Execute(Transform firePoint, Vector3 direction = default)
    {
        DropMeteor(firePoint, _enemy);
        AfterExecution();
    }

    public override void CalculateUtility(Enemy agent)
    {
        float CanSenseFactor = agent.PerceptionModule.CanSenseTarget ? MIN_UTILITY : 1f;
        float calculatedUtil = 0.5f * CanSenseFactor;

        SetUtilityWithModifiers(calculatedUtil);
    }

    private void DropMeteor(Transform firePoint, Enemy agent)
    {
        GameObject meteor = Instantiate(
            meteorPrefab,
            Player.Instance.Metrics.PredictNextPositionUsingMomentum() + (Vector3.up * 10),
            Quaternion.identity
        );
        Destroy(meteor, dropDelay + 1f);
    }
}
