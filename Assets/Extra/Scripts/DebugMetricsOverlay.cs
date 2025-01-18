#if UNITY_EDITOR
using TMPro;
using UnityEngine;

public class DebugMetricsOverlay : Singleton<DebugMetricsOverlay>
{
    public TextMeshProUGUI metricsText;
    private float updateInterval = 0.5f;
    private float fpsSamplePeriod = 2f;
    private float uptime;
    private float nextUpdateTime;
    private int frameCount;
    private float fpsAccumulator;
    private float currentFps;
    private float displayFps;

    protected override void Awake()
    {
        base.Awake();
        frameCount = 0;
        fpsAccumulator = 0f;
        currentFps = 0f;
        displayFps = 0f;
    }

    private void Update()
    {
        frameCount++;
        fpsAccumulator += Time.deltaTime;
        currentFps = 1f / Time.deltaTime;

        // Running average
        if (fpsAccumulator >= fpsSamplePeriod)
        {
            displayFps = frameCount / fpsAccumulator;
            frameCount = 0;
            fpsAccumulator = 0f;
        }
        else
        {
            displayFps = Mathf.Lerp(displayFps, currentFps, 0.1f);
        }

        if (Time.time < nextUpdateTime)
            return;

        nextUpdateTime = Time.time + updateInterval;
        uptime += updateInterval;
        UpdateMetrics();
    }

    private void UpdateMetrics()
    {
        if (!AgentManager.Instance || !AgentManager.Instance.playerAgent)
            return;

        Vector3 playerPos = AgentManager.Instance.playerAgent.transform.position;
        float playerHealth = AgentManager
            .Instance.playerAgent.GetModule<HealthModule>()
            .CurrentHealth;

        string metrics = string.Format(
            "FPS:{0:F1} | UP:{1:00}:{2:00} | POS:<color=#00ff00>{3:F0},{4:F0},{5:F0}</color> | "
                + "HP:{6:F0} | ENEMIES:{7} | ALLIES:{8}",
            displayFps,
            (int)uptime / 60,
            (int)uptime % 60,
            playerPos.x,
            playerPos.y,
            playerPos.z,
            playerHealth,
            AgentManager.Instance.activeEnemies.Count,
            AgentManager.Instance.activeAllies.Count
        );

        metricsText.text = metrics;
    }
}

#endif
