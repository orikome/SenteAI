using UnityEngine;

[CreateAssetMenu(fileName = "SeeingModule", menuName = "SenteAI/Modules/SeeingModule")]
public class SeeingModule : SenseModule
{
    [SerializeField]
    private float _range = 50f;
    private LayerMask _layerMask;
    private bool _wasVisible;
    private readonly float _cooldownTime = 0.5f;
    private float _lastVisibilityChangeTime;

    public override void Initialize(Agent agent)
    {
        _layerMask = OrikomeUtils.LayerMaskUtils.CreateMask("Player", "Wall", "Enemy");
    }

    public override void Execute(Agent agent)
    {
        if (agent.Target == null)
        {
            CanSenseTarget = false;
            return;
        }
        Vector3 directionToTarget = agent.Target.transform.position - agent.transform.position;
        Ray ray = new(agent.transform.position, directionToTarget.normalized);
        PlayerMetrics playerMetrics = (PlayerMetrics)AgentManager.Instance.playerAgent.Metrics;

        bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, _range, _layerMask);
        bool isVisible = hit && hitInfo.transform == agent.Target;

        DebugRay(agent, directionToTarget, isVisible, hit, hitInfo);

        // Return early if still in cooldown period
        if (Time.time - _lastVisibilityChangeTime < _cooldownTime)
            return;

        // Check if visibility has changed
        bool visibilityChanged = isVisible != _wasVisible;

        // Update if visible
        if (isVisible && visibilityChanged)
        {
            CanSenseTarget = true;
            playerMetrics.UpdateCoverStatus(true);
            LastKnownPosition = agent.Target.transform.position;
            LastKnownVelocity = AgentManager.Instance.playerAgent.Metrics.Velocity;
            LastSeenTime = Time.time;
            _lastVisibilityChangeTime = Time.time;
        }

        // Update if target was lost
        if (!isVisible && visibilityChanged)
        {
            CanSenseTarget = false;
            playerMetrics.UpdateCoverStatus(false);
            _lastVisibilityChangeTime = Time.time;
        }

        // Update visibility bool
        _wasVisible = isVisible;
    }

    private void DebugRay(
        Agent agent,
        Vector3 directionToTarget,
        bool targetVisible,
        bool hit,
        RaycastHit hitInfo
    )
    {
        float rayLength = hit ? hitInfo.distance : _range;

        Debug.DrawRay(
            agent.transform.position,
            directionToTarget.normalized * rayLength,
            targetVisible ? Color.green : Color.red
        );
    }
}
