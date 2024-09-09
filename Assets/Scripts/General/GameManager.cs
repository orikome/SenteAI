using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class TestData
{
    public int TimesTestRun;
    public float AverageDamageDone;
    public float AverageTimeAlive;
    public List<RunData> Runs = new List<RunData>();
}

[Serializable]
public class RunData
{
    public float DamageDone;
    public float TimeAlive;
}

[DefaultExecutionOrder(-1000)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public List<Agent> activeAgents = new();
    private string filePath;
    private TestData testData;

    void Awake()
    {
        Instance = this;
        filePath = Path.Combine(Application.persistentDataPath, "TestData.json");
        LoadOrInitializeMetrics();
    }

    void Start()
    {
        foreach (Agent agent in activeAgents)
        {
            agent.Initialize();
        }
    }

    void LoadOrInitializeMetrics()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            testData = JsonUtility.FromJson<TestData>(json);
        }
        else
        {
            testData = new TestData();
        }
    }

    public void RestartScene()
    {
        float timeAlive = Player.Instance.Metrics.TimeAlive;
        float damageDone = Player.Instance.Metrics.DamageDone;

        testData.TimesTestRun++;
        testData.Runs.Add(new RunData { DamageDone = damageDone, TimeAlive = timeAlive });

        float totalDamageDone = 0f;
        float totalTimeAlive = 0f;
        foreach (var run in testData.Runs)
        {
            totalDamageDone += run.DamageDone;
            totalTimeAlive += run.TimeAlive;
        }

        testData.AverageDamageDone = totalDamageDone / testData.TimesTestRun;
        testData.AverageTimeAlive = totalTimeAlive / testData.TimesTestRun;

        DebugManager.Instance.Log(
            $"DamageDone: {damageDone:F2} - TimeAlive: {timeAlive:F2} - AverageDamageDone: {testData.AverageDamageDone:F2} - AverageTimeAlive: {testData.AverageTimeAlive:F2}"
        );

        SaveMetricsToJson();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void SaveMetricsToJson()
    {
        try
        {
            string json = JsonUtility.ToJson(testData, true);
            File.WriteAllText(filePath, json);
        }
        catch (IOException ex)
        {
            Debug.LogError("Failed to write to JSON file: " + ex.Message);
        }
    }
}
