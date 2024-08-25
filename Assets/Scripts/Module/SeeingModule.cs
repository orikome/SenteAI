using UnityEngine;

[CreateAssetMenu(fileName = "SeeingModule", menuName = "Module/SeeingModule")]
public class SeeingModule : SenseModule
{
    [SerializeField]
    private float range = 10f;
    private LayerMask layerMask;
    private bool wasVisible;
    private float cooldownTime = 0.5f;
    private float lastVisibilityChangeTime;

    public override void Initialize(Agent agent)
    {
        layerMask = OrikomeUtils.LayerMaskUtils.CreateMask("Player", "Wall", "Enemy");
    }

    public override void ExecuteLoop(Agent agent)
    {
        Vector3 directionToTarget = agent.Target.position - agent.transform.position;
        Ray ray = new Ray(agent.transform.position, directionToTarget.normalized);

        bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, range, layerMask);
        bool isVisible = hit && hitInfo.transform == agent.Target;

        DebugRay(agent, directionToTarget, isVisible, hit, hitInfo);

        // Return early if still in cooldown period
        if (Time.time - lastVisibilityChangeTime < cooldownTime)
            return;

        // Check if visibility has changed
        bool visibilityChanged = isVisible != wasVisible;

        // Update if visible
        if (isVisible && visibilityChanged)
        {
            CanSenseTarget = true;
            Player.Instance.PlayerMetrics.UpdateCoverStatus(true);
            LastKnownLocation = agent.Target.position;
            LastSeen = Time.time;
            agent.ActionUtilityManager.ResetUtilityScores();
            Player.Instance.PlayerMetrics.timeInCover = 0;
            lastVisibilityChangeTime = Time.time;
        }

        // Update if target was lost
        if (!isVisible && visibilityChanged)
        {
            CanSenseTarget = false;
            Player.Instance.PlayerMetrics.UpdateCoverStatus(false);
            Player.Instance.PlayerMetrics.timeInCover = 0;
            lastVisibilityChangeTime = Time.time;
        }

        // Update visibility bool
        wasVisible = isVisible;
    }

    private void DebugRay(
        Agent agent,
        Vector3 directionToTarget,
        bool targetVisible,
        bool hit,
        RaycastHit hitInfo
    )
    {
        float rayLength = hit ? hitInfo.distance : range;

        Debug.DrawRay(
            agent.transform.position,
            directionToTarget.normalized * rayLength,
            targetVisible ? Color.green : Color.red
        );
    }
}
