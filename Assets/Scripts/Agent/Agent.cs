using System.Collections;
using System.Collections.Generic;
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
}
