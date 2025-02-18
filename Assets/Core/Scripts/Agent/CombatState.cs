using System.Linq;
using UnityEngine;

namespace SenteAI.Core
{
    public class CombatState : IAgentState
    {
        public void Enter(Agent agent)
        {
            agent.SetState(AgentState.Combat);
            Debug.Log("Entering Combat state", agent.gameObject);
        }

        public void Execute(Agent agent)
        {
            foreach (var module in agent.Modules.Where(m => m.agentState == AgentState.Combat))
            {
                module.Execute();
            }

            // Check for transition to idle
            if (agent.Target == null)
            {
                agent.TransitionToState(new IdleState());
            }
        }

        public void Exit(Agent agent)
        {
            Debug.Log("Exiting Combat state", agent.gameObject);
        }
    }
}
