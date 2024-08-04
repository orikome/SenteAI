using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AgentData", menuName = "Game/AgentData")]
public class AgentData : ScriptableObject
{
    public string agentName;
    public int maxHealth = 100;
    public List<AgentModule> modules;
    public List<AgentAction> actions;
    public ActionSelectionStrategy actionSelectionStrategy;
}
