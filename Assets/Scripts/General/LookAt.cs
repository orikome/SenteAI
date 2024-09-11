using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform model;
    public Transform target;
    public float rotationSpeed = 60f;
    public float idleRotationSpeed = 10f;
    EnemyAgent _agent;

    void Start()
    {
        _agent = gameObject.GetComponent<EnemyAgent>();
    }

    private void Update()
    {
        if (_agent.PerceptionModule.CanSenseTarget)
        {
            LookAtTransform(target, rotationSpeed);
        }
        else
        {
            // PanTowardsPredictedPosition(
            //     _agent.GetModule<SeeingModule>().LastKnownVelocity
            // );
            LookAtTransform(target, rotationSpeed / 8);
        }
    }

    public void LookAtTransform(Transform point, float rotSpeed)
    {
        Vector3 direction = (point.position - model.position).normalized;
        RotateTowards(direction, rotSpeed);
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
