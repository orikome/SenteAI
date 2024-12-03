using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Agent : MonoBehaviour
{
    // -- Set these in editor --
    public AgentData Data;
    public Transform firePoint;

    // -- These are set in code --
    public List<Module> Modules { get; private set; } = new();
    public List<AgentAction> Actions { get; private set; } = new();
    public Metrics Metrics { get; private set; }
    public Transform Target { get; protected set; }
    public AgentState State { get; protected set; }
    private Dictionary<System.Type, Module> _moduleCache = new();

    public virtual void Initialize()
    {
        // Initialize data and other components
        LoadAgentData();
        InitModules();
        InitActions();
        AddMetricsComponent();
        SelectTarget();
        // TODO: Should check and ensure that enemy has navMeshAgent
    }

    private void AddMetricsComponent()
    {
        switch (Data.faction)
        {
            case Faction.Player:
                Metrics = EnsureComponent<PlayerMetrics>();
                gameObject.tag = "Player";
                break;

            case Faction.Enemy:
                Metrics = EnsureComponent<EnemyMetrics>();
                gameObject.tag = "Enemy";
                break;

            case Faction.Neutral:
                gameObject.tag = "Neutral";
                break;
        }
    }

    public virtual void Update()
    {
        foreach (var module in Modules)
        {
            module.Execute(this);
        }
    }

    protected void SelectTarget()
    {
        if (Data.faction == Faction.Player)
        {
            Target = GameManager.Instance.activeEnemies.FirstOrDefault().transform;
        }
        else
        {
            Target = GameManager.Instance.playerAgent.transform;
        }
    }

    void OnEnable()
    {
        if (Data.faction == Faction.Enemy)
            GameManager.Instance.activeEnemies.Add(this);
    }

    void OnDisable()
    {
        if (Data.faction == Faction.Enemy)
            GameManager.Instance.activeEnemies.Remove(this);
    }

    public virtual void LoadAgentData()
    {
        // Ensure we only use data from our AgentData file
        Modules.Clear();
        Actions.Clear();
        _moduleCache.Clear();

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

        transform.gameObject.name = Data.agentName;
    }

    public void InitModules()
    {
        if (Modules.Count == 0)
            DebugManager.Instance.LogError("No modules assigned!");
        // Initialize modules
        foreach (var module in Modules)
        {
            module.Initialize(this);
            _moduleCache[module.GetType()] = module;
        }
    }

    public void InitActions()
    {
        if (Actions.Count == 0)
            DebugManager.Instance.LogError("No actions assigned!");
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

    public T EnsureComponent<T>()
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
}
