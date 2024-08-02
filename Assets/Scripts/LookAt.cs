using UnityEngine;

public class LookAt : MonoBehaviour
{
    public Transform model;
    public Transform target;
    public float rotationSpeed = 60f;

    private void Update()
    {
        LookAtTransform(target);
    }

    public void LookAtTransform(Transform point)
    {
        Vector3 direction = (point.position - model.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        model.rotation = Quaternion.RotateTowards(
            model.rotation,
            lookRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
