using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Agent
{
    public EnemyMetrics Metrics { get; private set; }

    public override void Initialize()
    {
        base.Initialize();

        // Ensure all components exist
        Metrics = EnsureComponent<EnemyMetrics>();
    }

    void OnEnable()
    {
        GameManager.Instance.activeEnemies.Add(this);
    }

    void OnDisable()
    {
        GameManager.Instance.activeEnemies.Remove(this);
    }
}
