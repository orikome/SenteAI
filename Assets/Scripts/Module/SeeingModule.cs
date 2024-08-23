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

    public override void Initialize()
    {
        layerMask = OrikomeUtils.LayerMaskUtils.CreateMask("Player", "Wall", "Enemy");
    }

    public override void ExecuteLoop(Agent agent)
    {
        Vector3 directionToTarget = agent.target.position - agent.transform.position;
        Ray ray = new Ray(agent.transform.position, directionToTarget.normalized);

        bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, range, layerMask);
        bool isVisible = hit && hitInfo.transform == agent.target;

        DebugRay(agent, directionToTarget, isVisible);

        // Return early if still in cooldown period
        if (Time.time - lastVisibilityChangeTime < cooldownTime)
            return;

        // Check if visibility has changed
        bool visibilityChanged = isVisible != wasVisible;

        // Update if visible
        if (isVisible && visibilityChanged)
        {
            CanSenseTarget = true;
            Player.Instance.playerMetrics.UpdateCoverStatus(true);
            LastKnownLocation = agent.target.position;
            LastSeen = Time.time;
            agent.actionUtilityManager.ResetUtilityScores();
            Player.Instance.playerMetrics.timeInCover = 0;
            lastVisibilityChangeTime = Time.time;
        }

        // Update if target was lost
        if (!isVisible && visibilityChanged)
        {
            CanSenseTarget = false;
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
}
