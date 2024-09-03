using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "CooldownBasedReadinessModule",
    menuName = "Module/CooldownBasedReadinessModule"
)]
public class CooldownBasedActionReadiness : ActionReadinessModule
{
    public float cooldownDuration = 1.0f;
    private Dictionary<AgentAction, float> lastActionTime = new Dictionary<AgentAction, float>();

    public override bool CanPerformAction(AgentAction action)
    {
        if (!lastActionTime.ContainsKey(action))
            return true;
        return Time.time - lastActionTime[action] >= cooldownDuration;
    }

    public override void OnActionPerformed(AgentAction action) { }

    public override void ExecuteLoop(Agent agent) { }

    public override void Initialize(Agent agent) { }
}
