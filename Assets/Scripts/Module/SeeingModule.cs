using UnityEngine;

[CreateAssetMenu(fileName = "SeeingModule", menuName = "Module/SeeingModule")]
public class SeeingModule : PerceptionModule
{
    [SerializeField]
    private float range = 10f;

    [SerializeField]
    private LayerMask layerMask;
    public bool canSeeTarget { get; private set; }
    private bool wasVisible;
    private float cooldownTime = 0.5f;
    private float lastVisibilityChangeTime;

    private void OnValidate()
    {
        if (layerMask == 0)
            Debug.LogWarning("LayerMask is empty!", this);
    }

    public override void Execute(Agent agent)
    {
        target = Player.Instance.transform;
        Vector3 directionToTarget = target.position - agent.transform.position;
        Ray ray = new Ray(agent.transform.position, directionToTarget.normalized);

        bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, range, layerMask);
        bool isVisible = hit && hitInfo.transform == target;

        DebugRay(agent, directionToTarget, isVisible);

        // Return early if still in cooldown period
        if (Time.time - lastVisibilityChangeTime < cooldownTime)
            return;

        // Check if visibility has changed
        bool visibilityChanged = isVisible != wasVisible;

        // Update if visible
        if (isVisible && visibilityChanged)
        {
            canSeeTarget = true;
            Player.Instance.playerMetrics.UpdateCoverStatus(true);
            lastKnownLocation = target.position;
            lastSeen = Time.time;
            agent.actionWeightManager.ResetWeights();
            Player.Instance.playerMetrics.timeInCover = 0;
            lastVisibilityChangeTime = Time.time;
        }

        // Update if target was lost
        if (!isVisible && visibilityChanged)
        {
            canSeeTarget = false;
            Player.Instance.playerMetrics.UpdateCoverStatus(false);
            Player.Instance.playerMetrics.timeInCover = 0;
            lastVisibilityChangeTime = Time.time;
        }

        // Update visibility bool
        wasVisible = isVisible;
    }

    private void DebugRay(Agent agent, Vector3 directionToTarget, bool targetVisible)
    {
        Debug.DrawRay(
            agent.transform.position,
            directionToTarget.normalized * range,
            targetVisible ? Color.green : Color.red
        );
    }

    public override void Initialize(AgentEvents events) { }
}
