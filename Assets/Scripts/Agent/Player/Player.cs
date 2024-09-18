using UnityEngine;

public class Player : Agent
{
    public static Player Instance { get; private set; }
    public PlayerMetrics Metrics { get; private set; }
    public KeyCode selectionKey = KeyCode.Mouse0;

    void Awake()
    {
        Instance = this;
    }

    public override void Initialize()
    {
        Metrics = EnsureComponent<PlayerMetrics>();
        LoadAgentData();
        InitModules();
        InitActions();
    }

    public bool IsInputHeld()
    {
        return Input.GetKey(selectionKey);
    }

    public Vector3 GetShootDirection()
    {
        var nearestEnemy = Metrics.FindClosestEnemyToPlayer();
        if (nearestEnemy != null)
        {
            return (nearestEnemy.position - firePoint.position).normalized;
        }

        return firePoint.forward;
    }
}
