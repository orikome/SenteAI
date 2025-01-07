using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Metrics))]
public class Agent : MonoBehaviour
{
    // -- Set these in editor --
    public AgentData data;
    public Transform firePoint;

    // -- These are set in code --
    public List<Module> Modules { get; private set; } = new();
    public List<AgentAction> Actions { get; private set; } = new();
    public Metrics Metrics { get; private set; }
    public Agent Target { get; protected set; }
    public AgentState State { get; protected set; }
    public Faction Faction { get; protected set; }
    private readonly Dictionary<System.Type, Module> _moduleCache = new();

    public virtual void Initialize()
    {
        LoadAgentData();
        InitializeModules();
        InitializeActions();
        SelectTarget();
    }

    public virtual void Update()
    {
        foreach (var module in Modules)
        {
            module.Execute();
        }

        SelectTarget();
    }

    void OnEnable()
    {
        Initialize();
        AgentManager.Instance.RegisterAgent(this);
    }

    void OnDisable()
    {
        AgentManager.Instance.UnregisterAgent(this);
    }

    private Agent FindClosestTarget(List<Agent> potentialTargets)
    {
        if (potentialTargets == null || potentialTargets.Count == 0)
            return null;

        Vector3 myPosition = transform.position;
        Agent closestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (Agent target in potentialTargets)
        {
            if (target == null || !target.gameObject.activeInHierarchy)
                continue;

            float distance = OrikomeUtils.GeneralUtils.GetDistanceSquared(
                myPosition,
                target.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        return closestTarget;
    }

    protected void SelectTarget()
    {
        if (Time.frameCount % 32 != 0)
            return;

        switch (Faction)
        {
            case Faction.Player:
            case Faction.Ally:
                Target = FindClosestTarget(AgentManager.Instance.activeEnemies);
                break;

            case Faction.Enemy:
                var potentialTargets = new List<Agent>(AgentManager.Instance.activeAllies);
                potentialTargets.Add(AgentManager.Instance.playerAgent);
                Target = FindClosestTarget(potentialTargets);
                break;

            case Faction.Neutral:
                Target = null;
                break;
        }
    }

    public virtual void LoadAgentData()
    {
        // Ensure we only use data from our AgentData file
        Modules.Clear();
        Actions.Clear();
        _moduleCache.Clear();

        if (data == null)
        {
            AgentLogger.LogError("AgentData is not assigned!");
            Destroy(gameObject);
            return;
        }

        // Set faction, tag, layer, target and metrics
        Metrics = GetComponent<Metrics>();
        Faction = data.faction;
        gameObject.tag = Faction.ToString();
        Helpers.SetLayerRecursively(gameObject, LayerMask.NameToLayer(Faction.ToString()));
        Metrics.Initialize(this);

        // Add modules
        foreach (var module in data.modules)
        {
            if (module != null)
            {
                Module newModule = Instantiate(module);
                Modules.Add(newModule);
            }
        }

        // Add actions
        foreach (var action in data.actions)
        {
            if (action != null)
            {
                AgentAction newAction = Instantiate(action);
                Actions.Add(newAction);
            }
        }

        // Check if NPC has NavMeshAgentModule
        if (Faction != Faction.Player)
        {
            if (GetModule<NavMeshAgentModule>() == null)
            {
                AgentLogger.LogWarning("Agent is missing NavMeshAgentModule, is it intentional?");
            }
        }

        transform.gameObject.name = data.agentName + "[" + gameObject.GetInstanceID() + "]";
    }

    public void InitializeModules()
    {
        if (Modules.Count == 0)
            AgentLogger.LogError("No modules assigned!");
        // Initialize modules
        foreach (var module in Modules)
        {
            module.Initialize(this);
            _moduleCache[module.GetType()] = module;
        }
    }

    public void InitializeActions()
    {
        if (Actions.Count == 0)
            AgentLogger.LogError("No actions assigned!");
        // Initialize actions
        foreach (var action in Actions)
        {
            action.Initialize(this);
        }
    }

    public T GetModule<T>()
        where T : Module
    {
        if (_moduleCache.TryGetValue(typeof(T), out var cachedModule))
            return cachedModule as T;

        T module = Modules.OfType<T>().FirstOrDefault();
        if (module != null)
            _moduleCache[typeof(T)] = module;

        return module;
    }
}
