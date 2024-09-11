using System.Collections;
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

    public virtual void Update()
    {
        foreach (var module in Modules)
        {
            module.Execute(null);
        }
    }

    public virtual void LoadAgentData()
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

        transform.gameObject.name = Data.agentName;
    }

    public void InitModules()
    {
        // Initialize modules
        foreach (var module in Modules)
        {
            module.Initialize(this);
        }
    }

    public T GetModule<T>()
        where T : Module
    {
        return Modules.OfType<T>().FirstOrDefault();
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
