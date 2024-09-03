using UnityEngine;

public class CircularMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float circleRadius = 5.0f;
    public float pauseDuration = 1.0f;
    public float pauseInterval = 2.0f;

    private float pauseTimer = 0f;
    private bool isPaused = false;
    private float angle = 0f;

    private void Start()
    {
        transform.position = new Vector3(circleRadius, 0, 0);
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

        angle += moveSpeed * Time.deltaTime / circleRadius;

        angle %= 2 * Mathf.PI;

        float x = Mathf.Cos(angle) * circleRadius;
        float z = Mathf.Sin(angle) * circleRadius;
        transform.position = new Vector3(x, 0, z);

        pauseTimer -= Time.deltaTime;
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
