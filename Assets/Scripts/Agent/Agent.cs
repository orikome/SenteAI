using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(
    typeof(NavMeshAgent),
    typeof(AgentActionUtilityManager),
    typeof(AgentActionDecisionMaker)
)]
public class Agent : MonoBehaviour
{
    // Set these in the editor
    public AgentData Data;
    public Transform firePoint;

    // These are set in code
    public AgentActionUtilityManager ActionUtilityManager { get; private set; }
    public AgentActionDecisionMaker ActionDecisionMaker { get; private set; }
    public ActionReadinessModule ReadinessModule { get; private set; }
    public SenseModule PerceptionModule { get; private set; }
    public ActionSelectionStrategy ActionSelectionStrategy { get; private set; }
    public AgentEvents Events { get; private set; }
    public List<AgentModule> Modules { get; private set; } = new();
    public Transform Target { get; private set; }
    public AgentContext Context { get; private set; }
    public float distanceToPlayer; // Set by playerMetrics
    private NavMeshAgent _navMeshAgent;

    public void Initialize()
    {
        Context = new AgentContext
        {
            DistanceToPlayer = distanceToPlayer,
            HealthFactor = 0.5f,
            EnergyLevel = 0.5f
        };

        Debug.Log("AgentContext Initialized: " + Context);

        Target = Player.Instance.transform;
        _navMeshAgent = GetComponent<NavMeshAgent>();
        ActionUtilityManager = GetComponent<AgentActionUtilityManager>();
        ActionDecisionMaker = GetComponent<AgentActionDecisionMaker>();
        Events = GetComponent<AgentEvents>();

        // Ensure we only use data from our AgentData file
        Modules.Clear();
        ActionUtilityManager.actions.Clear();
        ActionSelectionStrategy = null;

        InitializeData();
        ActionDecisionMaker.Initialize(this);
        ActionUtilityManager.Initialize();

        ReadinessModule = GetModule<ActionReadinessModule>();
        PerceptionModule = GetModule<SenseModule>();

        Debug.Assert(firePoint != null, "FirePoint is not set!");
        Debug.Assert(ReadinessModule != null, "ActionReadinessModule is not assigned!");
        Debug.Assert(GetModule<HealthModule>() != null, "HealthModule is not assigned!");
        Debug.Assert(PerceptionModule != null, "PerceptionModule is not assigned!");
        Debug.Assert(ActionSelectionStrategy != null, "ActionSelectionStrategy is not assigned!");
        Debug.Assert(Events != null, "AgentEvents is not assigned!");

        if (ActionUtilityManager.actions.Count == 0)
        {
            Debug.LogError("No actions assigned!");
        }

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

        AgentAction decidedAction = ActionDecisionMaker.MakeDecision();
        decidedAction?.ExecuteLoop(firePoint, this);
    }

    private void InitializeData()
    {
        if (Data == null)
        {
            Debug.LogError("AgentData is not assigned!");
            return;
        }

        // Initialize modules
        foreach (var module in Data.modules)
        {
            if (module != null)
            {
                AgentModule newModule = Instantiate(module);
                Modules.Add(newModule);
            }
        }

        // Initialize actions
        foreach (var action in Data.actions)
        {
            if (action != null)
            {
                AgentAction newAction = Instantiate(action);
                ActionUtilityManager.actions.Add(newAction);
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
}
