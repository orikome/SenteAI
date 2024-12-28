using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(
    fileName = "NPCMovementExperimental",
    menuName = "SenteAI/Modules/NPCMovementExperimental"
)]
public class NPCMovementExperimental : Module
{
    private readonly float _moveRadius = 20f;
    private readonly int _samples = 10;
    private Agent _nonPlayerAgent;
    private bool _isJumping = false;
    private float _jumpForce = 10f;
    private float _maxJumpHeight = 4f; // Increased from 3f
    private float _jumpDetectionRange = 5f; // Increased range
    private Rigidbody _rb;
    private NavMeshAgent _navMeshAgent;
    private float _jumpStartTime;
    private float _previousYPosition;
    private float _landingThreshold = 0.2f; // Increased threshold
    private float _lastNavToggleTime;
    private bool _hasStableLanding = false;
    private float _stableTimeRequired = 0.5f;
    private float _stableStartTime;

    public override void Initialize(Agent agent)
    {
        _nonPlayerAgent = agent;
        _rb = agent.GetComponent<Rigidbody>();
        _navMeshAgent = agent.GetComponent<NavMeshAgent>();

        if (_rb != null)
        {
            _rb.constraints = RigidbodyConstraints.FreezeRotation; // Prevent tumbling
            _rb.useGravity = true;
            _rb.isKinematic = false;
        }

        if (_rb == null || _navMeshAgent == null)
        {
            Debug.LogError("Missing required components on agent!");
        }
    }

    public override void Execute(Agent agent)
    {
        if (_nonPlayerAgent.Target == null)
            return;

        Vector3 targetPosition = _nonPlayerAgent.Target.transform.position;

        if (_isJumping)
        {
            float currentYPosition = _nonPlayerAgent.transform.position.y;
            float yDifference = Mathf.Abs(currentYPosition - _previousYPosition);

            // Check for initial landing
            if (
                !_hasStableLanding
                && yDifference < _landingThreshold
                && _rb.linearVelocity.y < 0.1f
            )
            {
                _stableStartTime = Time.time;
                _hasStableLanding = true;
            }

            // Confirm stable landing
            if (_hasStableLanding && Time.time > _stableStartTime + _stableTimeRequired)
            {
                Debug.Log("Stable landing confirmed");
                _isJumping = false;
                _hasStableLanding = false;
                _rb.linearVelocity = Vector3.zero;
                _lastNavToggleTime = Time.time;
                _navMeshAgent.enabled = true;
                _navMeshAgent.SetDestination(targetPosition);
            }

            _previousYPosition = currentYPosition;
            return;
        }

        // Check if we can use NavMesh path first
        NavMeshPath path = new NavMeshPath();
        if (_navMeshAgent.enabled && _navMeshAgent.CalculatePath(targetPosition, path))
        {
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                _navMeshAgent.SetDestination(targetPosition);
                return;
            }
        }

        // Only consider jumping if no valid path exists
        float heightDifference = targetPosition.y - _nonPlayerAgent.transform.position.y;

        if (heightDifference < -1.0f)
        {
            RaycastHit hit;
            Vector3 landingCheckStart = targetPosition + Vector3.up;
            if (Physics.Raycast(landingCheckStart, Vector3.down, out hit, 3f))
            {
                if (IsPathClear(targetPosition))
                {
                    InitiateJump(targetPosition);
                    return;
                }
            }
        }

        if (IsJumpNeeded(targetPosition))
        {
            InitiateJump(targetPosition);
        }
    }

    private bool IsJumpNeeded(Vector3 targetPosition)
    {
        float heightDifference = targetPosition.y - _nonPlayerAgent.transform.position.y;

        // Check if we need to jump down
        if (heightDifference < -0.5f)
        {
            RaycastHit hit;
            if (Physics.Raycast(targetPosition + Vector3.up, Vector3.down, out hit, 10f))
            {
                Debug.Log($"Need to jump down. Height difference: {heightDifference}");
                return true;
            }
        }

        // Original upward jump check
        RaycastHit obstacleHit;
        Vector3 directionToTarget = (
            targetPosition - _nonPlayerAgent.transform.position
        ).normalized;

        if (
            Physics.Raycast(
                _nonPlayerAgent.transform.position + Vector3.up * 0.5f,
                directionToTarget,
                out obstacleHit,
                _jumpDetectionRange
            )
        )
        {
            float obstacleHeight = obstacleHit.point.y - _nonPlayerAgent.transform.position.y;
            return obstacleHeight > 0.5f && obstacleHeight < _maxJumpHeight;
        }

        return false;
    }

    private void InitiateJump(Vector3 targetPosition)
    {
        _isJumping = true;
        _jumpStartTime = Time.time;
        _navMeshAgent.enabled = false;

        Vector3 jumpDirection = (targetPosition - _nonPlayerAgent.transform.position).normalized;
        float heightDifference = targetPosition.y - _nonPlayerAgent.transform.position.y;

        Vector3 jumpVector;
        if (heightDifference < -0.5f) // Jumping down
        {
            jumpVector = (jumpDirection * _jumpForce * 0.5f) + (Vector3.up * _jumpForce * 0.3f);
        }
        else // Jumping up/forward
        {
            jumpVector = (jumpDirection * _jumpForce) + (Vector3.up * _jumpForce);
        }

        _rb.linearVelocity = Vector3.zero; // Clear existing velocity
        _rb.AddForce(jumpVector, ForceMode.Impulse);
    }

    private bool IsPathClear(Vector3 targetPosition)
    {
        RaycastHit hit;
        Vector3 direction = (targetPosition - _nonPlayerAgent.transform.position).normalized;
        return !Physics.Raycast(
            _nonPlayerAgent.transform.position,
            direction,
            out hit,
            Vector3.Distance(_nonPlayerAgent.transform.position, targetPosition)
        );
    }

    private bool IsAgentOnNavMesh()
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(
            _nonPlayerAgent.transform.position,
            out hit,
            0.1f,
            NavMesh.AllAreas
        );
    }

    private void HandleJump()
    {
        // Wait for downward motion
        if (_rb.linearVelocity.y >= 0.1f)
            return;

        if (IsAgentOnNavMesh())
        {
            Debug.Log("Agent landed on NavMesh");
            _isJumping = false;
            _rb.linearVelocity = Vector3.zero;
            _navMeshAgent.enabled = true;

            if (_nonPlayerAgent.Target != null)
            {
                _navMeshAgent.SetDestination(_nonPlayerAgent.Target.transform.position);
            }
        }
    }

    private void CheckLanding()
    {
        // Only check landing when moving downward
        if (_rb.linearVelocity.y > 0.1f)
        {
            Debug.Log("Still ascending...");
            return;
        }

        if (IsAgentOnNavMesh())
        {
            Debug.Log("Landing detected on NavMesh");
            _isJumping = false;
            _rb.linearVelocity = Vector3.zero;
            _navMeshAgent.enabled = true;

            if (_nonPlayerAgent.Target != null)
            {
                _navMeshAgent.SetDestination(_nonPlayerAgent.Target.transform.position);
            }
        }
    }

    private Vector3 EvaluateBestPosition(Agent agent, Vector3 predictedTargetPosition)
    {
        Vector3 bestPosition = Vector3.zero;
        float bestScore = float.MinValue;

        Vector3 sampleCenter;

        switch (_nonPlayerAgent.Target.Metrics.CurrentBehavior)
        {
            case Behavior.Aggressive:
                // When the target is aggressive, sample positions around the agent to find cover
                sampleCenter = agent.transform.position;
                break;
            case Behavior.Defensive:
                // When the target is defensive, sample positions around the player to approach them
                sampleCenter = predictedTargetPosition;
                break;
            case Behavior.Balanced:
                // Sample positions between the agent and the target
                sampleCenter = Vector3.Lerp(
                    agent.transform.position,
                    predictedTargetPosition,
                    0.5f
                );
                break;
            default:
                sampleCenter = agent.transform.position;
                break;
        }

        for (int i = 0; i < _samples; i++)
        {
            Vector3 randomPoint = sampleCenter + Random.insideUnitSphere * _moveRadius;
            if (
                NavMesh.SamplePosition(
                    randomPoint,
                    out NavMeshHit hit,
                    _moveRadius,
                    NavMesh.AllAreas
                )
            )
            {
                Vector3 samplePosition = hit.position;
                float score = ScorePosition(samplePosition, agent, predictedTargetPosition);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestPosition = samplePosition;
                }
            }
        }

        return bestPosition;
    }

    private float ScorePosition(Vector3 position, Agent agent, Vector3 predictedTargetPosition)
    {
        float score = 0f;

        float distanceToPredictedTargetPosition = Vector3.Distance(
            position,
            predictedTargetPosition
        );

        bool positionInCover = IsInCover(position);

        switch (_nonPlayerAgent.Target.Metrics.CurrentBehavior)
        {
            case Behavior.Aggressive:
                // Target is aggressive; enemy should seek cover
                if (positionInCover)
                    score += 20f; // Encourage positions in cover
                else
                    score -= 10f; // Discourage positions not in cover

                // Maintain a safe distance
                score += Mathf.Clamp(distanceToPredictedTargetPosition, 10f, 30f);
                break;

            case Behavior.Defensive:
                // Target is defensive; enemy should approach
                score -= distanceToPredictedTargetPosition; // Encourage getting closer

                // Slightly prefer positions in cover
                if (positionInCover)
                    score += 5f;
                break;

            case Behavior.Balanced:
                // Neutral behavior
                score -= Mathf.Abs(distanceToPredictedTargetPosition - 15f); // Prefer moderate distance

                // Slight preference for cover
                if (positionInCover)
                    score += 10f;
                break;
        }

        // Check for line of sight to the player
        if (HasLineOfSight(position, predictedTargetPosition))
        {
            score += 5f; // Bonus for visibility
        }
        else
        {
            score -= 5f; // Penalty for lack of visibility
        }

        return score;
    }

    private bool HasLineOfSight(Vector3 fromPosition, Vector3 targetPosition)
    {
        if (Physics.Raycast(fromPosition, targetPosition - fromPosition, out RaycastHit hit))
            return hit.transform == _nonPlayerAgent.Target.transform;

        return false;
    }

    private bool IsInCover(Vector3 position)
    {
        return !HasLineOfSight(position, _nonPlayerAgent.Target.transform.position);
    }
}
