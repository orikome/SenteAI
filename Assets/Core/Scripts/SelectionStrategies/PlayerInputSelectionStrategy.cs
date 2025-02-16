using System.Linq;
using SenteAI.Extra;
using UnityEngine;

namespace SenteAI.Core
{
    [CreateAssetMenu(
        fileName = "PlayerInputSelectionStrategy",
        menuName = "SenteAI/SelectionStrategies/PlayerInputSelection"
    )]
    public class PlayerInputSelectionStrategy : ActionSelectionStrategy
    {
        public KeyCode selectionKey = KeyCode.Mouse0;

        public override AgentAction SelectAction(Agent agent)
        {
            if (Player.Instance.IsInputHeld())
            {
                Player.Instance.PlayerWeaponRecoil.TriggerRecoil();
                return agent.Actions.FirstOrDefault();
            }

            return null;
        }
    }
}
