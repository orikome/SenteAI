using UnityEngine;

[CreateAssetMenu(fileName = "SeeingModule", menuName = "SenteAI/Modules/SeeingModule")]
public class SeeingModule : SenseModule
{
    [SerializeField]
    private readonly float _range = 250f;
    private LayerMask _layerMask;
    public bool HasLOS { get; private set; }

    public override void Initialize(Agent agent)
    {
        base.Initialize(agent);
        _layerMask = OrikomeUtils.LayerMaskUtils.CreateMask("Player", "Wall", "Enemy", "Ally");
    }

    public override void Execute()
    {
        // Reset states if no target
        if (_agent == null || _agent.Target == null)
        {
            CanSenseTarget = false;
            HasLOS = false;
            return;
        }

        // Check LOS
        bool isVisible = IsTargetVisible(_agent.Metrics.GetDirectionToTarget());

        // Update states based on visibility
        CanSenseTarget = isVisible;
        HasLOS = isVisible;

        if (isVisible)
        {
            LastKnownPosition = _agent.Target.transform.position;
            LastKnownVelocity = _agent.Target.Metrics.Velocity;
            LastSeenTime = Time.time;
        }
    }

    private bool IsTargetVisible(Vector3 directionToTarget)
    {
        Ray ray = new(_agent.transform.position, directionToTarget);
        return Physics.Raycast(ray, out RaycastHit hitInfo, _range, _layerMask)
            && hitInfo.transform.gameObject == _agent.Target.transform.gameObject;
    }
}
