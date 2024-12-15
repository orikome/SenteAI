using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "NavMeshAgentModule", menuName = "SenteAI/Modules/NavMeshAgentModule")]
public class NavMeshAgentModule : Module
{
    public NavMeshAgent NavMeshAgent { get; private set; }
    public Vector3 CurrentDestination { get; private set; }

    public override void Execute(Agent agent) { }

    public override void Initialize(Agent agent)
    {
        if (agent.GetComponent<NavMeshAgent>() == null)
            agent.AddComponent<NavMeshAgent>();

        NavMeshAgent = agent.GetComponent<NavMeshAgent>();

        // Set NavMeshAgent properties
        NavMeshAgent.acceleration = 100;
        NavMeshAgent.angularSpeed = 50;
        NavMeshAgent.speed = 10;
        NavMeshAgent.autoBraking = false;
    }

    public void SetDestination(Vector3 destination)
    {
        NavMeshAgent.SetDestination(destination);
        CurrentDestination = destination;
    }
}
