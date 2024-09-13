public class Player : Agent
{
    public static Player Instance { get; private set; }
    public PlayerMetrics Metrics { get; private set; }

    void Awake()
    {
        Instance = this;
        Metrics = EnsureComponent<PlayerMetrics>();
        LoadAgentData();
        InitModules();
        InitActions();
    }
}
