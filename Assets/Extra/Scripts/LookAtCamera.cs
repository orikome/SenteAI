using SenteAI.Core;
using UnityEngine;

namespace SenteAI.Extra
{
    public class LookAtCamera : MonoBehaviour
    {
        Camera cameraToLookAt;

        void Start()
        {
            cameraToLookAt = Camera.main;
        }

        void LateUpdate()
        {
            if (cameraToLookAt != null)
            {
                transform.LookAt(cameraToLookAt.transform);
                transform.rotation = Quaternion.LookRotation(cameraToLookAt.transform.forward);
            }
        }
    }
}
