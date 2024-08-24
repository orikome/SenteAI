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
    public List<AgentModule> modules = new List<AgentModule>();

    [HideInInspector]
    public AgentActionUtilityManager actionUtilityManager;

    [HideInInspector]
    public AgentActionDecisionMaker actionDecisionMaker;

    [HideInInspector]
    public ActionReadinessModule readinessModule;

    [HideInInspector]
    public SenseModule perceptionModule;
    public ActionSelectionStrategy actionSelectionStrategy;

    [HideInInspector]
    public AgentEvents events;

    public AgentData data;
    public Transform firePoint;
    private NavMeshAgent navMeshAgent;
    public float distanceToPlayer;
    public Transform target;
    public AgentContext context;

    public void Initialize()
    {
        context = new AgentContext
        {
            DistanceToPlayer = distanceToPlayer,
            HealthFactor = 0.5f,
            EnergyLevel = 0.5f
        };

        Debug.Log("AgentContext Initialized: " + context);

        target = Player.Instance.transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        actionUtilityManager = GetComponent<AgentActionUtilityManager>();
        actionDecisionMaker = GetComponent<AgentActionDecisionMaker>();
        events = GetComponent<AgentEvents>();

        // Ensure we only use data from our AgentData file
        modules.Clear();
        actionUtilityManager.actions.Clear();
        actionSelectionStrategy = null;

        InitializeData();
        actionDecisionMaker.Initialize(this);
        actionUtilityManager.Initialize();

        readinessModule = GetModule<ActionReadinessModule>();
        perceptionModule = GetModule<SenseModule>();

        Debug.Assert(firePoint != null, "FirePoint is not set!");
        Debug.Assert(readinessModule != null, "ActionReadinessModule is not assigned!");
        Debug.Assert(GetModule<HealthModule>() != null, "HealthModule is not assigned!");
        Debug.Assert(perceptionModule != null, "PerceptionModule is not assigned!");
        Debug.Assert(actionSelectionStrategy != null, "ActionSelectionStrategy is not assigned!");
        Debug.Assert(events != null, "AgentEvents is not assigned!");

        if (actionUtilityManager.actions.Count == 0)
        {
            Debug.LogError("No actions assigned!");
        }

        // Initialize modules
        foreach (var module in modules)
        {
            module.Initialize(this);
        }

        // Initialize actions
        foreach (var action in actionUtilityManager.actions)
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
        foreach (var module in modules)
        {
            module.ExecuteLoop(this);
        }

        actionUtilityManager.CalculateUtilityScores();

        //DebugLog();

        AgentAction decidedAction = actionDecisionMaker.MakeDecision();
        decidedAction?.ExecuteLoop(firePoint, this);
    }

    private void InitializeData()
    {
        if (data == null)
        {
            Debug.LogError("AgentData is not assigned!");
            return;
        }

        // Initialize modules
        foreach (var module in data.modules)
        {
            if (module != null)
            {
                AgentModule newModule = Instantiate(module);
                modules.Add(newModule);
            }
        }

        // Initialize actions
        foreach (var action in data.actions)
        {
            if (action != null)
            {
                AgentAction newAction = Instantiate(action);
                actionUtilityManager.actions.Add(newAction);
            }
        }

        actionSelectionStrategy = data.actionSelectionStrategy;
    }

    private void DebugLog()
    {
        if (Time.frameCount % 300 != 0)
            return;

        string debugInfo = "";

        foreach (var action in actionUtilityManager.actions)
        {
            debugInfo += $"A: {action.name}, W: {action.utilityScore:F2}, C: {action.cost}\n";
        }
        DebugManager.Instance.Log(transform, debugInfo, Color.white);
    }

    public void SetDestination(Vector3 destination)
    {
        navMeshAgent.SetDestination(destination);
    }

    public T GetModule<T>()
        where T : AgentModule
    {
        return modules.OfType<T>().FirstOrDefault();
    }
}
