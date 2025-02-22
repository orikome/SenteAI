using SenteAI.Core;
using UnityEngine;
using UnityEngine.AI;

namespace SenteAI.Extra
{
    [CreateAssetMenu(
        fileName = "NavMeshAgentModule",
        menuName = "SenteAI/Modules/NavMeshAgentModule"
    )]
    public class NavMeshAgentModule : Module
    {
        public NavMeshAgent NavMeshAgent { get; private set; }
        public Vector3 CurrentDestination { get; private set; }
        private bool _isPaused = false;
        private float _pauseTimer = 0f;

        public void PauseFor(float duration)
        {
            _isPaused = true;
            _pauseTimer = duration;
            NavMeshAgent.isStopped = true;
        }

        public override void Execute()
        {
            if (_isPaused)
            {
                _pauseTimer -= Time.deltaTime;
                if (_pauseTimer <= 0f)
                {
                    _isPaused = false;
                    NavMeshAgent.isStopped = false;
                }
                return;
            }
        }

        public override void Initialize(Agent agent)
        {
            if (agent.GetComponent<NavMeshAgent>() == null)
                agent.gameObject.AddComponent<NavMeshAgent>();

            NavMeshAgent = agent.GetComponent<NavMeshAgent>();

            // Set NavMeshAgent properties
            NavMeshAgent.acceleration = 100;
            NavMeshAgent.angularSpeed = 50;
            //NavMeshAgent.speed = 10;
            NavMeshAgent.autoBraking = false;
        }

        public void SetDestination(Vector3 destination)
        {
            NavMeshAgent.SetDestination(destination);
            CurrentDestination = destination;
        }
    }
}
