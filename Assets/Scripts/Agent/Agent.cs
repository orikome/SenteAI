using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(
    typeof(NavMeshAgent),
    typeof(AgentActionWeightManager),
    typeof(AgentActionDecisionMaker)
)]
public class Agent : MonoBehaviour, IDamageable
{
    public List<AgentModule> modules = new List<AgentModule>();

    [HideInInspector]
    public AgentActionWeightManager actionWeightManager;

    [HideInInspector]
    public AgentActionDecisionMaker actionDecisionMaker;

    [HideInInspector]
    public ActionReadinessModule readinessModule;

    [HideInInspector]
    public PerceptionModule perceptionModule;
    public ActionSelectionStrategy actionSelectionStrategy;

    [HideInInspector]
    public AgentEvents events;

    public AgentData data;
    public Transform firePoint;
    private NavMeshAgent navMeshAgent;

    #region IDamageable Properties
    private float currentHealth;
    public float MaxHealth => data.maxHealth;
    public float CurrentHealth => currentHealth;
    #endregion


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        actionWeightManager = GetComponent<AgentActionWeightManager>();
        actionDecisionMaker = GetComponent<AgentActionDecisionMaker>();
        events = GetComponent<AgentEvents>();

        // Ensure we only use data from our AgentData file
        modules.Clear();
        actionWeightManager.actions.Clear();
        actionSelectionStrategy = null;

        InitializeData();
        currentHealth = MaxHealth;
        actionDecisionMaker.Initialize(this);
        actionWeightManager.Initialize();

        readinessModule = GetModule<ActionReadinessModule>();
        perceptionModule = GetModule<PerceptionModule>();

        // Register modules with events
        foreach (var module in modules)
        {
            module.RegisterEvents(events);
        }

        Debug.Assert(firePoint != null, "FirePoint is not set!");
        Debug.Assert(readinessModule != null, "ActionReadinessModule is not assigned!");
        Debug.Assert(perceptionModule != null, "PerceptionModule is not assigned!");
        Debug.Assert(actionSelectionStrategy != null, "ActionSelectionStrategy is not assigned!");
        Debug.Assert(events != null, "AgentEvents is not assigned!");

        if (actionWeightManager.actions.Count == 0)
        {
            Debug.LogError("No actions assigned!");
        }

        // Initialize actions
        foreach (var action in actionWeightManager.actions)
        {
            action.Initialize(this);
        }
    }

    void Start()
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
            module.Execute(this);
        }

        foreach (var action in actionWeightManager.actions)
        {
            action.UpdateWeights(this);
        }

        //DebugLog();

        AgentAction decidedAction = actionDecisionMaker.MakeDecision();
        decidedAction?.ExecuteAction(firePoint, this);
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
                actionWeightManager.actions.Add(newAction);
            }
        }

        actionSelectionStrategy = data.actionSelectionStrategy;
    }

    private void DebugLog()
    {
        string debugInfo = "";

        foreach (var actionProbability in actionWeightManager.weights)
        {
            debugInfo +=
                $"A: {actionProbability.Key.name}, W: {actionProbability.Value:F2}, C: {actionProbability.Key.cost}\n";
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

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
