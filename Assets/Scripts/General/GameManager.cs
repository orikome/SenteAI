using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public List<Agent> activeEnemies = new();
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
