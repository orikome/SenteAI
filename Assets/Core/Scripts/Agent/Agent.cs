using System.Collections.Generic;
using System.Linq;
using SenteAI.Extra;
using UnityEngine;

namespace SenteAI.Core
{
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
        private IAgentState _currentState;

        public virtual void Initialize()
        {
            LoadAgentData();
            InitializeModules();
            InitializeActions();
            SelectTarget();
            TransitionToState(new CombatState());
        }

        public virtual void Update()
        {
            SelectTarget();
            _currentState?.Execute(this);
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

        public void TransitionToState(IAgentState newState)
        {
            _currentState?.Exit(this);
            _currentState = newState;
            _currentState?.Enter(this);
        }

        public void SetState(AgentState newState)
        {
            State = newState;
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
                    AgentLogger.LogWarning(
                        "Agent is missing NavMeshAgentModule, is it intentional?"
                    );
                }
            }

            transform.gameObject.name = data.agentName + "[" + gameObject.GetInstanceID() + "]";
        }

        public void InitializeModules()
        {
            if (Modules.Count == 0)
                AgentLogger.LogError("No modules assigned!");

            // 1. Create module cache without initialization
            foreach (var module in Modules)
            {
                // Cache by concrete type
                System.Type concreteType = module.GetType();
                _moduleCache[concreteType] = module;

                // Cache by all base module types
                System.Type baseType = concreteType.BaseType;
                while (
                    baseType != null
                    && baseType != typeof(object)
                    && typeof(Module).IsAssignableFrom(baseType)
                )
                {
                    _moduleCache[baseType] = module;
                    baseType = baseType.BaseType;
                }
            }

            // 2. Initialize now that they can all find each other
            foreach (var module in Modules)
            {
                module.Initialize(this);
            }
        }

        public virtual Vector3 GetShootDirection()
        {
            if (Faction != Faction.Player)
                return transform.forward;
            else
                return Player.Instance.GetMouseLookDirection();
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
            System.Type type = typeof(T);
            if (_moduleCache.TryGetValue(type, out var cachedModule))
                return (T)cachedModule;

            return null;
        }
    }
}
