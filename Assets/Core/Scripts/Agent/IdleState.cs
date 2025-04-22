using System.Linq;
using UnityEngine;

namespace SenteAI.Core
{
    public class IdleState : IAgentState
    {
        public void Enter(Agent agent)
        {
            agent.SetState(AgentState.Idle);
            AgentLogger.Log("Entering Idle state", agent.gameObject);
        }

        public void Execute(Agent agent)
        {
            foreach (var module in agent.Modules.Where(m => m.agentState == AgentState.Idle))
            {
                module.OnUpdate();
            }

            // Check for transition to combat
            if (agent.Target != null)
            {
                agent.TransitionToState(new CombatState());
            }
        }

        public void Exit(Agent agent)
        {
            AgentLogger.Log("Exiting Idle state", agent.gameObject);
        }
    }
}
