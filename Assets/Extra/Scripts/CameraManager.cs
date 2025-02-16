using System.Collections;
using SenteAI.Core;
using UnityEngine;

namespace SenteAI.Extra
{
    public class CameraManager : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField]
        private Transform target;

        [SerializeField]
        private Vector3 offset = new Vector3(0, 15, -8);

        [SerializeField]
        private float smoothSpeed = 10f;

        [Header("View Settings")]
        [SerializeField]
        private float rotationX = 45f;

        [SerializeField]
        private float minZoom = 8f;

        [SerializeField]
        private float maxZoom = 20f;

        [SerializeField]
        private float zoomSpeed = 4f;

        [SerializeField]
        private float currentZoom = 15f;

        [Header("Cinematic Effect")]
        private float cinematicDuration = 0.6f;
        private float cinematicTimeScale = 0.2f;
        private float cinematicZoom = 5f;
        private Transform defaultTarget;
        private float returnDelay = 0.1f;
        private float originalSmoothing;
        private float targetZoom;

        private void Start()
        {
            if (target == null)
                AgentLogger.LogWarning("No target assigned to CameraManager!");

            defaultTarget = target;
            originalSmoothing = smoothSpeed;

            if (target != null)
            {
                transform.position = target.position + offset;
                transform.rotation = Quaternion.Euler(rotationX, 0, 0);
            }
        }

        public void TemporarilyTrackTarget(Transform newTarget, float followSpeed = 20f)
        {
            StartCoroutine(ExecuteCinematicSequence(newTarget, followSpeed));
        }

        private IEnumerator ExecuteCinematicSequence(Transform newTarget, float followSpeed)
        {
            float originalTimeScale = Time.timeScale;
            float originalZoom = currentZoom;
            float originalSpeed = smoothSpeed;
            Transform originalTarget = target;

            try
            {
                Time.timeScale = cinematicTimeScale;
                targetZoom = cinematicZoom;
                smoothSpeed = followSpeed;
                target = newTarget;

                yield return new WaitForSecondsRealtime(cinematicDuration);

                Time.timeScale = originalTimeScale;
                targetZoom = originalZoom;

                yield return new WaitForSeconds(returnDelay);

                target = originalTarget;
                smoothSpeed = originalSpeed;
            }
            finally
            {
                Time.timeScale = originalTimeScale;
                currentZoom = originalZoom;
                smoothSpeed = originalSpeed;
                target = originalTarget;
            }
        }

        private void LateUpdate()
        {
            if (target == null)
                return;

            // Handle zoom
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            currentZoom = Mathf.Clamp(currentZoom - scrollInput * zoomSpeed, minZoom, maxZoom);

            // Calculate position with zoom
            Vector3 desiredPosition =
                target.position
                + Quaternion.Euler(0, 0, 0) * new Vector3(0, offset.y, -currentZoom);

            // Smooth follow
            Vector3 smoothedPosition = Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothSpeed * Time.deltaTime
            );
            transform.position = smoothedPosition;

            // Maintain fixed rotation
            transform.rotation = Quaternion.Euler(rotationX, 0, 0);
        }
    }
}
