using UnityEngine;

public class PlayerMovement : MonoBehaviour, IDamageable
{
    public float moveSpeed = 5.0f;
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;
    private CharacterController controller;

    private Vector3 velocity;
    private bool isGrounded;

    public float MaxHealth => 20000f;
    public float CurrentHealth => 20000f;
    private float currentHealth;

    public static PlayerMovement Instance;

    private void Awake()
    {
        Instance = this;
        controller = gameObject.AddComponent<CharacterController>();
        currentHealth = MaxHealth;
    }

    private void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0f;
        }

        Move();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
