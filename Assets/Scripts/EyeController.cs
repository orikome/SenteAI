using UnityEngine;

public class EyeController : MonoBehaviour
{
    public Transform eyeModel;

    // Limit of rotation to prevent the eye looking inside the head
    public float eyeRotationLimit = 60f;
    public Transform Target;

    private void Start()
    {
        Target = PlayerMovement.Instance?.transform;
    }

    private void Update()
    {
        LookAt(Target);
    }

    public void LookAt(Transform point)
    {
        Vector3 direction = (point.position - eyeModel.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        eyeModel.rotation = Quaternion.RotateTowards(
            eyeModel.rotation,
            lookRotation,
            eyeRotationLimit
        );
    }
}
