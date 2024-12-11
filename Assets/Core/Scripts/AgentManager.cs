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

    public void RestartScene()
    {
        float timeAlive = playerAgent.GetModule<HealthModule>().TimeAlive;
        PlayerMetrics playerMetrics = (PlayerMetrics)playerAgent.Metrics;
        float damageDone = playerMetrics.DamageDone;

        TestManager.Instance.SaveMetrics(damageDone, timeAlive);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
