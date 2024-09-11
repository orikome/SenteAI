using UnityEngine;

public class Player : Agent
{
    public static Player Instance { get; private set; }
    public PlayerMetrics Metrics { get; private set; }

    void Awake()
    {
        Instance = this;
        Metrics = gameObject.GetComponent<PlayerMetrics>();
        LoadAgentData();
        InitModules();
    }

    public override void Update()
    {
        foreach (var module in Modules)
        {
            module.Execute(null);
        }
    }
}
