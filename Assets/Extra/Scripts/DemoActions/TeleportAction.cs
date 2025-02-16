using SenteAI.Core;
using UnityEngine;

namespace SenteAI.Extra
{
    [CreateAssetMenu(fileName = "TeleportAction", menuName = "SenteAI/Actions/TeleportAction")]
    public class TeleportAction : AgentAction
    {
        public override void Execute(Transform firePoint, Vector3 direction) { }
    }
}
