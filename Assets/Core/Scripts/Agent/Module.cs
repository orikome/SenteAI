using UnityEngine;

namespace SenteAI.Core
{
    public abstract class Module : ScriptableObject
    {
        protected Agent _agent;

        /// <summary>
        /// Called once in the object's Awake method.
        /// </summary>
        public virtual void Initialize(Agent agent)
        {
            if (agent == null)
            {
                AgentLogger.LogError("Agent cannot be null");
                return;
            }
            _agent = agent;
        }

        /// <summary>
        /// Called every frame in the object's Update method.
        /// </summary>
        public abstract void Execute();
    }
}
