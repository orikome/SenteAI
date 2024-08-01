using UnityEngine;

[CreateAssetMenu(fileName = "DialogueModule", menuName = "Module/DialogueModule")]
public class DialogueModule : AgentModule
{
    [SerializeField]
    private Transform target;

    public override void Execute(Agent agent) { }
}
