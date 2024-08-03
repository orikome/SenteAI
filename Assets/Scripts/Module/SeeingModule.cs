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

    public override void Execute(Agent agent)
    {
        target = Player.Instance.transform;
        Vector3 directionToTarget = target.position - agent.transform.position;
        Ray ray = new Ray(agent.transform.position, directionToTarget.normalized);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, range, layerMask))
        {
            canSeeTarget = hitInfo.transform == target;
            if (canSeeTarget)
            {
                lastKnownLocation = target.position;
            }
        }
        else
        {
            canSeeTarget = false;
        }

        // Reset weights when target visibility changes
        if (previousVisibility != canSeeTarget)
        {
            agent.actionWeightManager.ResetWeights();
        }

        previousVisibility = canSeeTarget;
    }
}
