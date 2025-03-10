using System.Collections.Generic;
using SenteAI.Extra;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SenteAI.Core
{
    [DefaultExecutionOrder(-1000)]
    public class AgentManager : Singleton<AgentManager>
    {
        public List<Agent> activeEnemies = new();
        public List<Agent> activeAllies = new();
        public Agent playerAgent;

        public void RegisterAgent(Agent agent)
        {
            switch (agent.data.faction)
            {
                case Faction.Enemy:
                    activeEnemies.Add(agent);
                    break;
                case Faction.Ally:
                    activeAllies.Add(agent);
                    break;
                case Faction.Player:
                    playerAgent = agent;
                    break;
            }
        }

        public void UnregisterAgent(Agent agent)
        {
            switch (agent.data.faction)
            {
                case Faction.Enemy:
                    activeEnemies.Remove(agent);
                    break;
                case Faction.Ally:
                    activeAllies.Remove(agent);
                    break;
                case Faction.Player:
                    playerAgent = null;
                    break;
            }
        }

        public void RestartScene()
        {
            float timeAlive = playerAgent.GetModule<HealthModule>().TimeAlive;
            Metrics metrics = playerAgent.Metrics;
            float damageDone = metrics.DamageDone;

            TestManager.Instance.SaveMetrics(damageDone, timeAlive);

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
