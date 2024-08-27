using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public List<Agent> activeAgents = new();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        foreach (Agent agent in activeAgents)
        {
            agent.Initialize();
        }
    }

    public void RestartScene()
    {
        Debug.Log(
            "DamageDone: "
                + Player.Instance.PlayerMetrics.DamageDone
                + "TimeAlive: "
                + Player.Instance.PlayerMetrics.TimeAlive
        );
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
