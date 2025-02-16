using UnityEngine;

namespace SenteAI.Core
{
    public abstract class ActionSelectionStrategy : ScriptableObject
    {
        public abstract AgentAction SelectAction(Agent agent);
    }
}
