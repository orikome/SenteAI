using SenteAI.Core;
using UnityEngine;

namespace SenteAI.Extra
{
    [CreateAssetMenu(fileName = "DialogueModule", menuName = "SenteAI/Modules/DialogueModule")]
    public class DialogueModule : Module
    {
        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            TriggerDialogue();
        }

        public override void Execute() { }

        private void TriggerDialogue()
        {
            if (CanvasManager.Instance != null)
                CanvasManager.Instance.SpawnDamageText(
                    _agent.transform,
                    "Boss has spawned!",
                    Color.yellow
                );
        }
    }
}
