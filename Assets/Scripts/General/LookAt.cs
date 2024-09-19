using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform model; // Head
    public Transform body;
    public Transform target;
    public float rotationSpeed = 60f;
    public float idleRotationSpeed = 10f;
    Agent _agent;

    void Start()
    {
        _agent = gameObject.GetComponent<Agent>();
        target = Player.Instance.transform;
    }

    private void Update()
    {
        if (_agent.GetModule<SenseModule>().CanSenseTarget)
        {
            LookAtTransform(target, rotationSpeed);
        }
        else
        {
            LookAtTransform(target, idleRotationSpeed);
        }
    }

    public void LookAtTransform(Transform point, float rotSpeed)
    {
        Vector3 direction = (point.position - model.position).normalized;

        RotateTowards(model, direction, rotSpeed);

        float angle = Vector3.SignedAngle(body.forward, direction, Vector3.up);

        if (Mathf.Abs(angle) > 30f)
        {
            Vector3 bodyDirection = point.position - body.position;
            bodyDirection.y = 0f;
            if (bodyDirection.sqrMagnitude > 0f)
            {
                bodyDirection.Normalize();
                RotateTowards(body, bodyDirection, rotSpeed / 2);
            }
        }
    }

    private void RotateTowards(Transform part, Vector3 direction, float speed)
    {
        if (part == body)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude == 0f)
                return;
            direction.Normalize();
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            Vector3 eulerAngles = lookRotation.eulerAngles;
            eulerAngles.x = 0f;
            eulerAngles.z = 0f;
            lookRotation = Quaternion.Euler(eulerAngles);

            part.rotation = Quaternion.RotateTowards(
                part.rotation,
                lookRotation,
                speed * Time.deltaTime
            );
        }
        else
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            part.rotation = Quaternion.RotateTowards(
                part.rotation,
                lookRotation,
                speed * Time.deltaTime
            );

            float headAngle = Vector3.SignedAngle(body.forward, model.forward, Vector3.up);
            if (Mathf.Abs(headAngle) > 30f)
            {
                float constrainedAngle = Mathf.Clamp(headAngle, -30f, 30f);
                model.rotation = Quaternion.Euler(
                    model.eulerAngles.x,
                    body.eulerAngles.y + constrainedAngle,
                    model.eulerAngles.z
                );
            }
        }
    }
}
