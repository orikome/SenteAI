using UnityEngine;

[CreateAssetMenu(fileName = "EnergyBasedReadinessModule", menuName = "Module/EnergyBasedReadiness")]
public class EnergyBasedReadinessModule : ActionReadinessModule
{
    public float energyRecoveryRate = 20.0f;
    public float maxEnergy = 100.0f;
    private float curEnergy;

    private void OnEnable()
    {
        curEnergy = maxEnergy;
    }

    public override bool CanPerformAction(AgentAction action)
    {
        return curEnergy >= action.cost;
    }

    public override void OnActionPerformed(AgentAction action)
    {
        curEnergy -= action.cost;
    }

    public override void Execute(Agent agent)
    {
        curEnergy = Mathf.Min(curEnergy + energyRecoveryRate * Time.deltaTime, maxEnergy);
    }

    public override void RegisterEvents(AgentEvents agentEvents) { }
}
