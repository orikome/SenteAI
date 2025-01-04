using UnityEngine;

public class LookAt : MonoBehaviour
{
    [Header("References")]
    public Transform head;
    public Transform body;

    [Header("Head Settings")]
    public float maxHeadRotationSpeed = 240f;
    public float headRotationSmoothTime = 0.1f;
    public float maxHeadAngle = 70f;

    [Header("Body Settings")]
    public float maxBodyRotationSpeed = 120f;
    public float bodyTurnThreshold = 45f;
    public float minBodyTurnAngle = 5f;
    private Vector3 currentHeadVelocity;
    private float currentHeadAngle;
    private Agent _agent;

    void Start()
    {
        _agent = GetComponent<Agent>();
    }

    void Update()
    {
        if (_agent == null || _agent.Target == null)
            return;

        UpdateRotations(_agent.Target.transform);
    }

    void UpdateRotations(Transform target)
    {
        Vector3 targetDirection = (target.position - head.position).normalized;
        Vector3 flatTargetDirection = new Vector3(
            targetDirection.x,
            0,
            targetDirection.z
        ).normalized;

        // Calculate desired head rotation first
        float targetHeadAngle = Vector3.SignedAngle(body.forward, targetDirection, Vector3.up);
        currentHeadAngle = Mathf.SmoothDampAngle(
            currentHeadAngle,
            Mathf.Clamp(targetHeadAngle, -maxHeadAngle, maxHeadAngle),
            ref currentHeadVelocity.y,
            headRotationSmoothTime
        );

        // Rotate head
        Quaternion headRotation = body.rotation * Quaternion.Euler(0, currentHeadAngle, 0);
        head.rotation = Quaternion.RotateTowards(
            head.rotation,
            headRotation,
            maxHeadRotationSpeed * Time.deltaTime
        );

        // Body follows if head angle exceeds threshold
        if (Mathf.Abs(currentHeadAngle) > bodyTurnThreshold)
        {
            float bodyRotationAngle = Vector3.SignedAngle(
                body.forward,
                flatTargetDirection,
                Vector3.up
            );

            if (Mathf.Abs(bodyRotationAngle) > minBodyTurnAngle)
            {
                Quaternion targetBodyRotation = Quaternion.LookRotation(flatTargetDirection);
                body.rotation = Quaternion.RotateTowards(
                    body.rotation,
                    targetBodyRotation,
                    maxBodyRotationSpeed * Time.deltaTime
                );
            }
        }
    }
}
