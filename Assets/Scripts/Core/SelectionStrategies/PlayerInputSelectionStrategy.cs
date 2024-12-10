using System.Linq;
using UnityEngine;

[CreateAssetMenu(
    fileName = "PlayerInputSelectionStrategy",
    menuName = "ActionSelection/PlayerInputSelection"
)]
public class PlayerInputSelectionStrategy : ActionSelectionStrategy
{
    public KeyCode selectionKey = KeyCode.Mouse0;

    public override AgentAction SelectAction(Agent agent)
    {
        if (InputManager.Instance.IsInputHeld())
            return agent.Actions.FirstOrDefault();

        return null;
    }

    public override Vector3 GetShootDirection(Agent agent)
    {
        return PlayerLook.Instance.GetMouseLookDirection();
    }
}
