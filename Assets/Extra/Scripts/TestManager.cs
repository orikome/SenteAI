using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

public class TestManager : MonoBehaviour
{
    private string filePath;
    private TestData testData;

    public static TestManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;

        filePath = Path.Combine(Application.persistentDataPath, "TestData.json");
        InitJSON();
    }

    private void InitJSON()
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

    public void SaveMetrics(float damageDone, float timeAlive)
    {
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

        AgentLogger.Log(
            $"DamageDone: {damageDone:F2} - TimeAlive: {timeAlive:F2} - AverageDamageDone: {testData.AverageDamageDone:F2} - AverageTimeAlive: {testData.AverageTimeAlive:F2}"
        );

        SaveMetricsToJSON();
    }

    private void SaveMetricsToJSON()
    {
        try
        {
            string json = JsonUtility.ToJson(testData, true);
            File.WriteAllText(filePath, json);
        }
        catch (IOException ex)
        {
            AgentLogger.LogError("Failed to write to JSON file: " + ex.Message);
        }
    }
}
