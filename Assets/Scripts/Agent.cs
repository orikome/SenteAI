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
        readinessModule = GetModule<ActionReadinessModule>();
        perceptionModule = GetModule<PerceptionModule>();
        actionDecisionMaker.Initialize(this);
        currentHealth = MaxHealth;
        Debug.Assert(firePoint != null, "FirePoint is not set!");
        Debug.Assert(readinessModule != null, "ActionReadinessModule is not assigned!");
        Debug.Assert(perceptionModule != null, "PerceptionModule is not assigned!");
        Debug.Assert(actionSelectionStrategy != null, "ActionSelectionStrategy is not assigned!");

        if (actionWeightManager.actions.Count == 0)
        {
            Debug.LogError("No actions assigned!");
        }
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

        AgentAction decidedAction = actionDecisionMaker.MakeDecision();
        decidedAction?.ExecuteAction(firePoint, this);
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
