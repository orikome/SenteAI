using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Agent
{
    public SenseModule PerceptionModule { get; private set; }
    public EnemyMetrics Metrics { get; private set; }
    public Vector3 CurrentDestination { get; private set; }
    private NavMeshAgent _navMeshAgent;

    public override void Initialize()
    {
        // Set target as player
        Target = Player.Instance.transform;

        // Ensure all components exist
        _navMeshAgent = EnsureComponent<NavMeshAgent>();
        Metrics = EnsureComponent<EnemyMetrics>();

        // Set NavMeshAgent properties
        _navMeshAgent.acceleration = 100;
        _navMeshAgent.angularSpeed = 50;
        _navMeshAgent.speed = 10;
        _navMeshAgent.autoBraking = false;

        // Initialize data and other components
        LoadAgentData();
        InitModules();
        InitActions();

        // Get modules
        PerceptionModule = GetModule<SenseModule>();
    }

    public void SetDestination(Vector3 destination)
    {
        _navMeshAgent.SetDestination(destination);
        CurrentDestination = destination;
    }

    void OnDrawGizmos()
    {
        if (!EditorApplication.isPlaying || _navMeshAgent == null || _navMeshAgent.path == null)
            return;

        Gizmos.color = Color.cyan;

        NavMeshPath path = _navMeshAgent.path;

        // Draw path
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
        }

        // Draw spheres at path corners
        Gizmos.color = Color.red;
        foreach (Vector3 corner in path.corners)
        {
            Gizmos.DrawSphere(corner, 0.2f);
        }
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
