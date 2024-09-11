using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAgent : Agent
{
    // -- These are set in code --
    public SenseModule PerceptionModule { get; private set; }
    public ActionSelectionStrategy ActionSelectionStrategy { get; private set; }
    public Transform Target { get; private set; }
    public AgentMetrics Metrics { get; private set; }
    private NavMeshAgent _navMeshAgent;
    private float _lastActionTime;
    private readonly float _globalCooldown = 0.4f;

    public void Initialize()
    {
        // Set target as player
        Target = Player.Instance.transform;

        // Ensure all components exist
        _navMeshAgent = EnsureComponent<NavMeshAgent>();
        Metrics = EnsureComponent<AgentMetrics>();

        // Set navmesh properties
        _navMeshAgent.acceleration = 100;
        _navMeshAgent.angularSpeed = 50;
        _navMeshAgent.speed = 10;
        _navMeshAgent.autoBraking = false;

        // Ensure we only use data from our AgentData file
        Modules.Clear();
        Actions.Clear();
        ActionSelectionStrategy = null;

        // Initialize data and other components
        LoadAgentData();

        // Reset utility scores
        foreach (AgentAction action in Actions)
        {
            action.ScaledUtilityScore = 1.0f / Actions.Count;
        }
        DebugManager.Instance.SpawnTextLog(transform, "Reset utilScores", Color.red);

        // Get modules
        PerceptionModule = GetModule<SenseModule>();

        if (Actions.Count == 0)
            DebugManager.Instance.LogError("No actions assigned!");

        // Initialize modules
        foreach (var module in Modules)
        {
            module.Initialize(this);
        }

        // Initialize actions
        foreach (var action in Actions)
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

    public override void Update()
    {
        foreach (var module in Modules)
        {
            module.Execute(this);
        }

        foreach (var action in Actions)
        {
            action.CalculateUtility(this);
        }

        SelectAndExecuteAction();
    }

    private void SelectAndExecuteAction()
    {
        if (Time.time < _lastActionTime + _globalCooldown)
        {
            //DebugManager.Instance.Log("Global cooldown active, waiting to select action...");
            return;
        }

        AgentAction decidedAction = ActionSelectionStrategy.SelectAction(this);
        if (decidedAction == null)
        {
            DebugManager.Instance.LogWarning("No valid action selected.");
            return;
        }

        DebugManager.Instance.Log(
            $"Selected: {Helpers.CleanName(decidedAction.name)} with utilScore: {decidedAction.ScaledUtilityScore}"
        );
        Metrics.AddActionToHistory(decidedAction);
        // DebugManager.Instance.SpawnTextLog(
        //     transform,
        //     $"{Helpers.CleanName(decidedAction.name)}={decidedAction.utilityScore:F2}",
        //     Color.cyan
        // );
        decidedAction.Execute(firePoint, this);
        _lastActionTime = Time.time;
    }

    public void SetActionSelectionStrategy(ActionSelectionStrategy strategy)
    {
        // If you need to change the strategy during runtime for some reason
        ActionSelectionStrategy = strategy;
    }

    public void NormalizeUtilityScores()
    {
        float sum = Actions.Sum(action => action.ScaledUtilityScore);
        //Debug.Log($"Total util sum before normalization: {sum}");
        float minScore = 0.01f;

        // Prevent division by zero
        if (sum == 0)
            return;

        foreach (AgentAction action in Actions)
        {
            // Scale by base utility to preserve differences
            action.ScaledUtilityScore = Mathf.Max(action.ScaledUtilityScore / sum, minScore);
        }

        // Ensure scores sum to exactly 1
        sum = Actions.Sum(action => action.ScaledUtilityScore);
        foreach (AgentAction action in Actions)
        {
            action.ScaledUtilityScore /= sum;
        }
    }

    private void LoadAgentData()
    {
        if (Data == null)
        {
            DebugManager.Instance.LogError("AgentData is not assigned!");
            return;
        }

        // Add modules
        foreach (var module in Data.modules)
        {
            if (module != null)
            {
                Module newModule = Instantiate(module);
                Modules.Add(newModule);
            }
        }

        // Add actions
        foreach (var action in Data.actions)
        {
            if (action != null)
            {
                AgentAction newAction = Instantiate(action);
                Actions.Add(newAction);
            }
        }

        ActionSelectionStrategy = Data.actionSelectionStrategy;
        transform.gameObject.name = Data.agentName;
    }

    public void SetDestination(Vector3 destination)
    {
        _navMeshAgent.SetDestination(destination);
    }

    public T GetModule<T>()
        where T : Module
    {
        return Modules.OfType<T>().FirstOrDefault();
    }

    private T EnsureComponent<T>()
        where T : Component
    {
        if (!TryGetComponent<T>(out var component))
        {
            DebugManager.Instance.LogWarning(
                $"Component of type {typeof(T).Name} was missing and has been added."
            );
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