using UnityEngine;

[CreateAssetMenu(fileName = "SeeingModule", menuName = "SenteAI/Modules/SeeingModule")]
public class SeeingModule : SenseModule
{
    [SerializeField]
    private float _range = 250f;
    private LayerMask _layerMask;
    private bool _wasVisible;
    private readonly float _updateTime = 0.25f;
    private float _lastVisibilityChangeTime;

    public override void Initialize(Agent agent)
    {
        base.Initialize(agent);
        _layerMask = OrikomeUtils.LayerMaskUtils.CreateMask("Player", "Wall", "Enemy", "Ally");
    }

    public override void Execute()
    {
        if (_agent.Target == null)
        {
            CanSenseTarget = false;
            return;
        }
        Vector3 directionToTarget = _agent.Target.transform.position - _agent.transform.position;
        Ray ray = new(_agent.transform.position, directionToTarget.normalized);

        bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, _range, _layerMask);

        // Crucial for visibility check, gameObject comparison!
        bool isVisible = hit && hitInfo.transform.gameObject == _agent.Target.transform.gameObject;

        // Return early if still in cooldown period
        if (Time.time - _lastVisibilityChangeTime < _updateTime)
            return;

        // Check if visibility has changed
        bool visibilityChanged = isVisible != _wasVisible;

        // Update if visible
        if (isVisible && visibilityChanged)
        {
            CanSenseTarget = true;
            LastKnownPosition = _agent.Target.transform.position;
            LastKnownVelocity = _agent.Target.Metrics.Velocity;
            LastSeenTime = Time.time;
            _lastVisibilityChangeTime = Time.time;
        }

        // Update if target was lost
        if (!isVisible && visibilityChanged)
        {
            CanSenseTarget = false;
            _lastVisibilityChangeTime = Time.time;
        }

        // Update visibility bool
        _wasVisible = isVisible;
    }
}
