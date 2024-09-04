using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/MeteorStrike")]
public class MeteorStrikeAction : AgentAction
{
    public GameObject meteorPrefab;
    public float dropDelay = 2f;

    public override void Initialize(Agent agent) { }

    public override void ExecuteLoop(Transform firePoint, Agent agent)
    {
        DropMeteor(firePoint, agent);
        AddCooldown();
    }

    public override bool CanExecute(Agent agent)
    {
        return GetCooldownTimeRemaining() <= 0;
    }

    public override void CalculateUtility(Agent agent, AgentMetrics metrics)
    {
        float CanSenseFactor = agent.PerceptionModule.CanSenseTarget ? MIN_UTILITY : 1f;
        float calculatedUtil = 0.5f * CanSenseFactor;

        if (calculatedUtil <= 0)
            Debug.LogError(
                "Utility is zero or negative, check parameters: "
                    + "CanSense="
                    + agent.PerceptionModule.CanSenseTarget
            );

        //Debug.Log("Utility calculated: " + calculatedUtil);
        utilityScore = calculatedUtil;
    }

    private void DropMeteor(Transform firePoint, Agent agent)
    {
        GameObject meteor = Instantiate(
            meteorPrefab,
            Player.Instance.PlayerMetrics.PredictNextPositionUsingMomentum() + (Vector3.up * 10),
            Quaternion.identity
        );
        Destroy(meteor, dropDelay + 1f);
    }
}
