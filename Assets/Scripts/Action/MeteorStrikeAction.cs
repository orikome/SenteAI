using UnityEngine;

[CreateAssetMenu(menuName = "AgentAction/MeteorStrike")]
public class MeteorStrikeAction : AgentAction
{
    public GameObject meteorPrefab;
    public float dropDelay = 2f;

    public override void Execute(Transform firePoint, Agent agent)
    {
        DropMeteor(firePoint, agent);
        AfterExecution();
    }

    public override bool CanExecute(Agent agent)
    {
        return GetCooldownTimeRemaining() <= 0;
    }

    public override void CalculateUtility(Agent agent, AgentMetrics metrics)
    {
        float CanSenseFactor = agent.PerceptionModule.CanSenseTarget ? MIN_UTILITY : 1f;
        float calculatedUtil = 0.5f * CanSenseFactor;

        if (GetCooldownProgress() < 1.0f)
        {
            // If on cooldown, scaled by cooldown progress
            calculatedUtil *= GetCooldownProgress();
        }

        if (calculatedUtil <= 0)
            Debug.LogError(
                "Utility is zero or negative, check parameters: "
                    + "CanSense="
                    + agent.PerceptionModule.CanSenseTarget
            );

        //Debug.Log("Utility calculated: " + calculatedUtil);
        utilityScore = Mathf.Clamp(calculatedUtil, MIN_UTILITY, 1.0f);
    }

    private void DropMeteor(Transform firePoint, Agent agent)
    {
        GameObject meteor = Instantiate(
            meteorPrefab,
            Player.Instance.Metrics.PredictNextPositionUsingMomentum() + (Vector3.up * 10),
            Quaternion.identity
        );
        Destroy(meteor, dropDelay + 1f);
    }
}
