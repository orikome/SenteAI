using UnityEngine;

[CreateAssetMenu(fileName = "TeleportAction", menuName = "AgentAction/TeleportAction")]
public class TeleportAction : AgentAction
{
    public override void ExecuteActionLoop(Transform firePoint, Agent agent) { }

    public override void Initialize(Agent agent) { }
}
