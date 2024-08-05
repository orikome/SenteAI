using UnityEngine;

[CreateAssetMenu(fileName = "SeeingModule", menuName = "Module/SeeingModule")]
public class SeeingModule : PerceptionModule
{
    [SerializeField]
    private float range = 10f;

    [SerializeField]
    private LayerMask layerMask;
    public bool canSeeTarget { get; private set; }
    private bool previousVisibility;
    private float cooldownTime = 1f;
    private float lastVisibilityChangeTime;

    public override void Execute(Agent agent)
    {
        target = Player.Instance.transform;
        Vector3 directionToTarget = target.position - agent.transform.position;
        Ray ray = new Ray(agent.transform.position, directionToTarget.normalized);

        bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, range, layerMask);
        bool targetVisible = hit && hitInfo.transform == target;

        if (Time.time - lastVisibilityChangeTime < cooldownTime)
            return;

        canSeeTarget = targetVisible;

        if (targetVisible)
        {
            lastKnownLocation = target.position;
            lastSeen = Time.time;
        }

        // Reset weights when target visibility changes
        if (previousVisibility != canSeeTarget)
        {
            lastSeen = Time.time;
            //Debug.Log($"Target lost at: {Time.time}");
            lastVisibilityChangeTime = Time.time;
            agent.actionWeightManager.ResetWeights();
        }

        previousVisibility = canSeeTarget;
    }
}
