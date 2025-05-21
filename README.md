# SenteAI
![Unity Version](https://img.shields.io/badge/Unity-6000.0.36f1-blue)
![License](https://img.shields.io/badge/License-MIT-green)
> *Empower your game with a modular AI system that simplifies decision-making and adapts to player actions.*

SenteAI is an exploration of creating a dynamic, utility-based AI that can adapt to player actions. Each AI agent is composed of stackable `Module` components, which handle things such as health, vision, or the agent's brain for decision-making. 
These `Module` components, along with `Action` components, can be easily added or customized to adjust an agent's behavior, as they are both part of Unity's `ScriptableObject` class.

The name "Sente" (先手) is derived from Japanese, meaning "seizing the initiative" or "taking the first move." It reflects the idea behind the AI, where agents are proactive and continuously evaluating their environment to seize opportunities.

## ✨ Features

- **Modularity**: Easily stack `Module` components to create complex behaviors
- **Utility-Based Decision Making**: AI that adapts to changing game conditions
- **Selection Strategies**: Experiment with various decision-making algorithms
- **Visual Debugging Tools**: Real-time visualization of agent decision-making processes
- **Agent Factions**: Support for players, allies, enemies, and neutral entities


## 🛠️ Prerequisites
- Unity 6 (6000.0.36f1)
- Git

## 📚 Examples
The `Agent` base class is shared by both the `Player` and the `Enemy` for handling common functionality, such as executing `Module` components.
### Agent
```csharp
public class Agent : MonoBehaviour
{
    public List<Module> Modules { get; private set; } = new();
    public List<AgentAction> Actions { get; private set; } = new();

    public virtual void Update()
    {
        foreach (var module in Modules)
        {
            module.Execute(this);
        }
    }
}
```
### Brain
Both the `Player` and the `Enemy` have a brain `Module`, which handles the execution of `Action` components such as attacking or moving.
```csharp
public class Brain : Module
{
    public override void Execute(Agent agent)
    {
        if (!_cooldownHandler.IsReady())
            return;

        AgentAction decidedAction = ActionSelectionStrategy.SelectAction(agent);

        if (decidedAction != null)
        {
            decidedAction.Execute(
                agent.firePoint,
                ActionSelectionStrategy.GetShootDirection(agent)
            );
            _cooldownHandler.Reset();
        }
    }
}
```
