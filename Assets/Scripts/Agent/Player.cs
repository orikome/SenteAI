using UnityEngine;

public class Player : Agent
{
    public static Player Instance { get; private set; }
    public KeyCode selectionKey = KeyCode.Mouse0;
    public Agent CurrentlyControlledAgent;

    void Awake()
    {
        Instance = this;
    }

    public override void Initialize()
    {
        base.Initialize();
        //CurrentlyControlledAgent = this;
    }

    public bool IsInputHeld()
    {
        return Input.GetKey(selectionKey);
    }

    public Vector3 GetShootDirection()
    {
        PlayerMetrics playerMetrics = (PlayerMetrics)Metrics;
        var nearestEnemy = playerMetrics.FindClosestEnemyToPlayer();
        if (nearestEnemy != null)
        {
            return (nearestEnemy.position - firePoint.position).normalized;
        }

        return firePoint.forward;
    }
}
