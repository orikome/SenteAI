using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform model;
    public Transform target;
    public float rotationSpeed = 60f;
    public float idleRotationSpeed = 10f;
    Agent agent;

    void Start()
    {
        agent = gameObject.GetComponent<Agent>();
    }

    private void Update()
    {
        if (agent.PerceptionModule.CanSenseTarget)
        {
            LookAtTransform(target);
        }
        else
        {
            PanTowardsPredictedPosition(
                Player.Instance.PlayerMetrics.PredictNextPositionUsingAverage()
            );
        }
    }

    public void LookAtTransform(Transform point)
    {
        Vector3 direction = (point.position - model.position).normalized;
        RotateTowards(direction, rotationSpeed);
    }

    private void PanTowardsPredictedPosition(Vector3 predictedPosition)
    {
        if (predictedPosition == Vector3.zero)
            return;

        Vector3 directionToPredictedPosition = (predictedPosition - model.position).normalized;
        RotateTowards(directionToPredictedPosition, idleRotationSpeed);
    }

    private void RotateTowards(Vector3 direction, float speed)
    {
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        model.rotation = Quaternion.RotateTowards(
            model.rotation,
            lookRotation,
            speed * Time.deltaTime
        );
    }
}
