using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]
public class AgentManager : MonoBehaviour
{
    public static AgentManager Instance { get; private set; }
    public List<Agent> activeEnemies = new();
    public List<Agent> activeAllies = new();
    public Agent playerAgent;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerAgent.Initialize();

        foreach (Agent enemy in activeEnemies)
        {
            enemy.Initialize();
        }

        foreach (Agent ally in activeAllies)
        {
            ally.Initialize();
        }
    }

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
