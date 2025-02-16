using SenteAI.Core;
using UnityEngine;

namespace SenteAI.Extra
{
    [CreateAssetMenu(fileName = "PlayerMovement", menuName = "SenteAI/Modules/PlayerMovement")]
    public class PlayerMovement : Module
    {
        public float moveSpeed = 10.0f;
        private float jumpHeight = 6.0f;
        private float gravity = -30f;
        private CharacterController _controller;
        private Vector3 _velocity;
        private bool _isGrounded;

        // Dash
        public float dashSpeed = 40.0f;
        public float dashDuration = 0.2f;
        public float dashCooldown = 2.0f;
        private bool _isDashing;
        private float _dashTimeRemaining;
        private float _dashCooldownRemaining;
        private Vector3 _dashDirection;

        public override void Execute()
        {
            _isGrounded = _controller.isGrounded;
            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = 0f;
            }

            if (_dashCooldownRemaining > 0)
                _dashCooldownRemaining -= Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.LeftShift) && !_isDashing && _dashCooldownRemaining <= 0)
            {
                InitiateDash();
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
            if (_controller == null)
            {
                AgentLogger.LogError("CharacterController is not assigned!");
                return;
            }
        }

        private void InitiateDash()
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            Vector3 moveDirection = new Vector3(moveX, 0, moveZ);
            if (moveDirection.magnitude > 0)
            {
                _isDashing = true;
                _dashTimeRemaining = dashDuration;
                _dashDirection = moveDirection.normalized;
                _dashCooldownRemaining = dashCooldown;
            }
        }

        private void HandleMovement()
        {
            if (!_controller.enabled)
                return;

            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");
            Vector3 move = new(moveX, 0, moveZ);

            if (_isDashing)
            {
                _dashTimeRemaining -= Time.deltaTime;
                if (_dashTimeRemaining > 0)
                {
                    _controller.Move(dashSpeed * Time.deltaTime * _dashDirection);
                }
                else
                {
                    _isDashing = false;
                }
            }
            else
            {
                if (move.magnitude > 1f)
                    move = move.normalized;
                _controller.Move(moveSpeed * Time.deltaTime * move);
            }

            _velocity.y += gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }

        private void Jump()
        {
            _velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
