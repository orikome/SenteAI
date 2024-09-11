using System.Linq;
using UnityEngine;

[CreateAssetMenu(
    fileName = "PlayerInputSelectionStrategy",
    menuName = "ActionSelection/PlayerInputSelection"
)]
public class PlayerInputSelectionStrategy : ActionSelectionStrategy
{
    public KeyCode selectionKey = KeyCode.Mouse0;

    public override AgentAction SelectAction(Enemy agent)
    {
        return null;
    }

    public bool IsInputHeld()
    {
        return Input.GetKey(selectionKey);
    }
}
