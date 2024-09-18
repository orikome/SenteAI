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
    public Transform Target { get; protected set; }
    private Dictionary<System.Type, Module> _moduleCache = new();

    public virtual void Initialize()
    {
        // Initialize data and other components
        LoadAgentData();
        InitModules();
        InitActions();
    }

    public virtual void Update()
    {
        foreach (var module in Modules)
        {
            module.Execute(this);
        }
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
