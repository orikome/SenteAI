using SenteAI.Core;
using UnityEngine;

namespace SenteAI.Extra
{
    [System.Serializable]
    public class WaypointData
    {
        public Transform point;
        public float waitTime = 2f;
    }

    [CreateAssetMenu(fileName = "SplinePatrolModule", menuName = "SenteAI/Modules/Spline Patrol")]
    public class PatrolModule : Module
    {
        [Header("Path Settings")]
        [SerializeField]
        private WaypointData[] waypoints;

        [SerializeField]
        private float moveSpeed = 5f;

        [SerializeField]
        private float rotationSpeed = 2f;

        [SerializeField]
        private bool loop = true;

        private int currentWaypoint = 0;
        private float waitTimer = 0f;
        private float journeyTime = 0f;
        private bool isWaiting = false;

        private NavMeshAgentModule _navAgent;

        public override void Initialize(Agent agent)
        {
            base.Initialize(agent);
            _navAgent = agent.GetModule<NavMeshAgentModule>();

            if (_navAgent == null)
            {
                AgentLogger.LogError("NavMeshAgentModule is required for PatrolModule");
                return;
            }

            _navAgent.NavMeshAgent.speed = moveSpeed;
            _navAgent.NavMeshAgent.angularSpeed = rotationSpeed * 100f;

            // If waypoints are already set, skip random generation
            if (waypoints != null && waypoints.Length >= 2)
                return;

            // Create 3 random waypoints
            waypoints = new WaypointData[3];
            Vector3 agentPosition = agent.transform.position;
            float radius = 45f;

            GameObject waypointsParent = new GameObject("Patrol_Waypoints");
            waypointsParent.transform.position = agentPosition;

            for (int i = 0; i < 3; i++)
            {
                GameObject waypointObj = new GameObject($"Waypoint_{i}");

                float angle = i * (360f / 3) * Mathf.Deg2Rad;
                Vector3 position =
                    agentPosition
                    + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

                waypointObj.transform.position = position;
                waypoints[i] = new WaypointData { point = waypointObj.transform, waitTime = 2f };
                waypointObj.transform.SetParent(waypointsParent.transform);
            }

            AgentLogger.Log($"Created {waypoints.Length} random waypoints for patrol");
        }

        public override void OnUpdate()
        {
            if (isWaiting)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer >= waypoints[currentWaypoint].waitTime)
                {
                    isWaiting = false;
                    waitTimer = 0f;
                    MoveToNextWaypoint();
                }
            }
            else
            {
                MoveAlongPath();
            }
        }

        void MoveAlongPath()
        {
            journeyTime += Time.deltaTime;

            Vector3[] points = GetSplinePoints();
            float t = journeyTime;

            if (t > 1f)
            {
                isWaiting = true;
                journeyTime = 0f;
                return;
            }

            Vector3 currentSplinePos = CatmullRom(points[0], points[1], points[2], points[3], t);
            _navAgent.SetDestination(currentSplinePos);
        }

        Vector3[] GetSplinePoints()
        {
            Vector3[] points = new Vector3[4];

            if (loop)
            {
                points[0] = GetWaypointPosition(
                    (currentWaypoint - 1 + waypoints.Length) % waypoints.Length
                );
                points[1] = GetWaypointPosition(currentWaypoint);
                points[2] = GetWaypointPosition((currentWaypoint + 1) % waypoints.Length);
                points[3] = GetWaypointPosition((currentWaypoint + 2) % waypoints.Length);
            }
            else
            {
                points[0] = GetWaypointPosition(currentWaypoint - 1);
                points[1] = GetWaypointPosition(currentWaypoint);
                points[2] = GetWaypointPosition(currentWaypoint + 1);
                points[3] = GetWaypointPosition(currentWaypoint + 2);
            }

            return points;
        }

        void MoveToNextWaypoint()
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Length)
            {
                if (loop)
                    currentWaypoint = 0;
                else
                {
                    //_navAgent.Agent.enabled = false;
                    // Stop moving
                    _navAgent.SetDestination(_agent.transform.position);
                }
            }
        }

        Vector3 GetWaypointPosition(int index)
        {
            if (index < 0)
                return waypoints[0].point.position;
            if (index >= waypoints.Length)
                return waypoints[waypoints.Length - 1].point.position;
            return waypoints[index].point.position;
        }

        Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            return 0.5f
                * (
                    (-t3 + 2f * t2 - t) * p0
                    + (3f * t3 - 5f * t2 + 2f) * p1
                    + (-3f * t3 + 4f * t2 + t) * p2
                    + (t3 - t2) * p3
                );
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (waypoints == null || waypoints.Length < 2)
                return;

            float steps = 20f;
            int segments = loop ? waypoints.Length : waypoints.Length - 1;

            for (int i = 0; i < segments; i++)
            {
                Vector3[] points = new Vector3[4];

                points[0] = waypoints[(i - 1 + waypoints.Length) % waypoints.Length].point.position;
                points[1] = waypoints[i % waypoints.Length].point.position;
                points[2] = waypoints[(i + 1) % waypoints.Length].point.position;
                points[3] = waypoints[(i + 2) % waypoints.Length].point.position;

                for (float t = 0; t < 1f; t += 1f / steps)
                {
                    Vector3 p1 = CatmullRom(points[0], points[1], points[2], points[3], t);
                    Vector3 p2 = CatmullRom(
                        points[0],
                        points[1],
                        points[2],
                        points[3],
                        t + 1f / steps
                    );

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(p1, p2);
                }
            }

            foreach (var waypoint in waypoints)
            {
                if (waypoint?.point != null)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(waypoint.point.position, 0.5f);
                }
            }
        }
#endif
    }
}
