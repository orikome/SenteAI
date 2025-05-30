using System.Collections.Generic;
using UnityEngine;

namespace SenteAI.Core
{
    [CreateAssetMenu(fileName = "AgentData", menuName = "SenteAI/AgentData")]
    public class AgentData : ScriptableObject
    {
        public string agentName;
        public Faction faction;
        public int maxHealth = 100;
        public float actionCooldown = 0.4f;
        public List<Module> modules;
        public List<AgentAction> actions;
        public ActionSelectionStrategy actionSelectionStrategy;
    }
}
