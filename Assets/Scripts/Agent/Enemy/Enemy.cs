using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Agent
{
    public SenseModule PerceptionModule { get; private set; }
    public EnemyMetrics Metrics { get; private set; }
    public Vector3 CurrentDestination { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }

    public override void Initialize()
    {
        base.Initialize();

        // Set target as player
        Target = Player.Instance.transform;

        // Ensure all components exist
        NavMeshAgent = EnsureComponent<NavMeshAgent>();
        Metrics = EnsureComponent<EnemyMetrics>();

        // Set NavMeshAgent properties
        NavMeshAgent.acceleration = 100;
        NavMeshAgent.angularSpeed = 50;
        NavMeshAgent.speed = 10;
        NavMeshAgent.autoBraking = false;

        // Get modules
        PerceptionModule = GetModule<SenseModule>();
    }

    public void SetDestination(Vector3 destination)
    {
        NavMeshAgent.SetDestination(destination);
        CurrentDestination = destination;
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
