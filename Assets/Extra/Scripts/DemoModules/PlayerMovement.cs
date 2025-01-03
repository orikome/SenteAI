using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMovement", menuName = "SenteAI/Modules/PlayerMovement")]
public class PlayerMovement : Module
{
    public float moveSpeed = 10.0f;
    private float jumpHeight = 6.0f;
    public float gravity = -9.81f;
    private CharacterController _controller;
    private Vector3 _velocity;
    private bool _isGrounded;

    public override void Execute()
    {
        if (_controller == null)
        {
            AgentLogger.LogError("CharacterController is not assigned!");
            return;
        }
        _isGrounded = _controller.isGrounded;
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = 0f;
        }

        HandleMovement();

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            Jump();
        }
    }

    public override void Initialize(Agent agent)
    {
        base.Initialize(agent);
        _controller = agent.gameObject.GetComponent<CharacterController>();
    }

    private void HandleMovement()
    {
        if (!_controller.enabled)
            return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new(moveX, 0, moveZ);
        if (move.magnitude > 1f)
            move = move.normalized;
        _controller.Move(moveSpeed * Time.deltaTime * move);
        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void Jump()
    {
        _velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
}
