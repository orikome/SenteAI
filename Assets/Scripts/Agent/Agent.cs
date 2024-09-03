using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(AgentUtilityManager))]
public class Agent : MonoBehaviour
{
    // Set these in editor
    public AgentData Data;
    public Transform firePoint;

    // These are set in code
    public AgentUtilityManager ActionUtilityManager { get; private set; }
    public ActionReadinessModule ReadinessModule { get; private set; }
    public SenseModule PerceptionModule { get; private set; }
    public ActionSelectionStrategy ActionSelectionStrategy { get; private set; }
    public AgentEvents Events { get; private set; }
    public List<AgentModule> Modules { get; private set; } = new();
    public Transform Target { get; private set; }
    public AgentMetrics AgentMetrics { get; private set; }
    private NavMeshAgent _navMeshAgent;

    public void Initialize()
    {
        // Set target as player
        Target = Player.Instance.transform;

        // Ensure all components exist
        _navMeshAgent = EnsureComponent<NavMeshAgent>();
        ActionUtilityManager = EnsureComponent<AgentUtilityManager>();
        Events = EnsureComponent<AgentEvents>();
        AgentMetrics = EnsureComponent<AgentMetrics>();

        // Set navmesh properties
        _navMeshAgent.acceleration = 100;
        _navMeshAgent.angularSpeed = 50;
        _navMeshAgent.speed = 10;
        _navMeshAgent.autoBraking = false;

        // Ensure we only use data from our AgentData file
        Modules.Clear();
        ActionUtilityManager.ClearActions();
        ActionSelectionStrategy = null;

        // Initialize data and other components
        LoadAgentData();
        ActionUtilityManager.Initialize();

        // Get modules
        ReadinessModule = GetModule<ActionReadinessModule>();
        PerceptionModule = GetModule<SenseModule>();

        if (ActionUtilityManager.actions.Count == 0)
            Debug.LogError("No actions assigned!");

        // Initialize modules
        foreach (var module in Modules)
        {
            module.Initialize(this);
        }

        // Initialize actions
        foreach (var action in ActionUtilityManager.actions)
        {
            action.Initialize(this);
        }
    }

    void OnEnable()
    {
        GameManager.Instance.activeAgents.Add(this);
    }

    void OnDisable()
    {
        GameManager.Instance.activeAgents.Remove(this);
    }

    private void Update()
    {
        foreach (var module in Modules)
        {
            module.ExecuteLoop(this);
        }

        ActionUtilityManager.CalculateUtilityScores();

        //DebugLog();

        AgentAction decidedAction = ActionSelectionStrategy.SelectAction(this);
        if (decidedAction)
        {
            AgentMetrics.actionHistory.Add(decidedAction);
            decidedAction?.ExecuteLoop(firePoint, this);
        }
    }

    public void SetActionSelectionStrategy(ActionSelectionStrategy strategy)
    {
        // If you need to change the strategy during runtime for some reason
        ActionSelectionStrategy = strategy;
    }

    private void LoadAgentData()
    {
        if (Data == null)
        {
            Debug.LogError("AgentData is not assigned!");
            return;
        }

        // Add modules
        foreach (var module in Data.modules)
        {
            if (module != null)
            {
                AgentModule newModule = Instantiate(module);
                Modules.Add(newModule);
            }
        }

        // Add actions
        foreach (var action in Data.actions)
        {
            if (action != null)
            {
                AgentAction newAction = Instantiate(action);
                ActionUtilityManager.AddAction(newAction);
            }
        }

        ActionSelectionStrategy = Data.actionSelectionStrategy;
    }

    private void DebugLog()
    {
        if (Time.frameCount % 300 != 0)
            return;

        string debugInfo = "";

        foreach (var action in ActionUtilityManager.actions)
        {
            debugInfo += $"A: {action.name}, W: {action.utilityScore:F2}, C: {action.cost}\n";
        }
        DebugManager.Instance.Log(transform, debugInfo, Color.white);
    }

    public void SetDestination(Vector3 destination)
    {
        _navMeshAgent.SetDestination(destination);
    }

    public T GetModule<T>()
        where T : AgentModule
    {
        return Modules.OfType<T>().FirstOrDefault();
    }

    private T EnsureComponent<T>()
        where T : Component
    {
        if (!TryGetComponent<T>(out var component))
        {
            Debug.LogWarning($"Component of type {typeof(T).Name} was missing and has been added.");
            component = gameObject.AddComponent<T>();
        }
        return component;
    }

    void OnDrawGizmos()
    {
        if (!EditorApplication.isPlaying || _navMeshAgent.path == null)
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
}
