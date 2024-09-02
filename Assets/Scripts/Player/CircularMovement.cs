using UnityEngine;

public class CircularMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float circleRadius = 5.0f;
    public float pauseDuration = 1.0f;
    public float pauseInterval = 2.0f;

    private float pauseTimer = 0f;
    private bool isPaused = false;
    private Vector3 centerPoint;

    private void Start()
    {
        transform.position = new Vector3(circleRadius, 0, 0);
        centerPoint = Vector3.zero;
    }

    private void Update()
    {
        if (isPaused)
        {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                isPaused = false;
                pauseTimer = pauseInterval;
            }
            return;
        }

        float angularVelocity = moveSpeed / circleRadius;
        float angle = angularVelocity * Time.deltaTime;

        transform.RotateAround(centerPoint, Vector3.up, angle * Mathf.Rad2Deg);

        if (pauseTimer <= 0f)
        {
            isPaused = true;
            pauseTimer = pauseDuration;
        }
        else
        {
            pauseTimer -= Time.deltaTime;
        }
    }
}
