using System.Collections;
using UnityEditor.Analytics;
using UnityEngine;

[CreateAssetMenu(menuName = "SenteAI/Actions/NPCExplosiveMelee")]
public class NPCExplosiveMelee : AgentAction
{
    public GameObject warningIndicator;
    public GameObject explosionPrefab;
    public GameObject impactParticlePrefab;
    Animator _animator;
    public float explosionRadius = 14f;
    public int explosionDamage = 20;
    private float rayCastDistance = 16f;
    public float raySpreadAngle = 40f;
    private LayerMask _obstacleLayer;
    private LayerMask _targetMask;
    private LayerMask _ownerMask;

    public override void Initialize(Agent agent)
    {
        base.Initialize(agent);
        //_animator = agent.GetComponent<Animator>(); // Should be cached in the agent
        _targetMask = Helpers.GetTargetMask(_agent.Faction);
        _ownerMask = Helpers.GetOwnerMask(_agent.Faction);
        _obstacleLayer = LayerMask.GetMask("Wall");
    }

    private bool IsSurroundingClear(Transform firePoint, Vector3 direction)
    {
        // Calculate the three ray directions
        Vector3 leftRay = Quaternion.Euler(0, -raySpreadAngle / 2, 0) * direction;
        Vector3 centerRay = direction;
        Vector3 rightRay = Quaternion.Euler(0, raySpreadAngle / 2, 0) * direction;

        // Cast all three rays
        bool leftClear = !Physics.Raycast(
            firePoint.position,
            leftRay,
            rayCastDistance,
            _obstacleLayer
        );
        bool centerClear = !Physics.Raycast(
            firePoint.position,
            centerRay,
            rayCastDistance,
            _obstacleLayer
        );
        bool rightClear = !Physics.Raycast(
            firePoint.position,
            rightRay,
            rayCastDistance,
            _obstacleLayer
        );

        // Optional: Debug visualization
        Debug.DrawRay(
            firePoint.position,
            leftRay * rayCastDistance,
            leftClear ? Color.green : Color.red,
            1f
        );
        Debug.DrawRay(
            firePoint.position,
            centerRay * rayCastDistance,
            centerClear ? Color.green : Color.red,
            1f
        );
        Debug.DrawRay(
            firePoint.position,
            rightRay * rayCastDistance,
            rightClear ? Color.green : Color.red,
            1f
        );

        // Return true only if all rays are clear
        return leftClear && centerClear && rightClear;
    }

    public override void Execute(Transform firePoint, Vector3 direction)
    {
        StartMelee(firePoint);
        AfterExecution();
    }

    public override void CalculateUtility()
    {
        float utility = new UtilityBuilder()
            .WithMeleeDistance(_agent.Metrics.DistanceToTarget, optimalRange: 5f, maxRange: 15f)
            .WithAggression(_agent.GetModule<HealthModule>().CurrentHealth, maxHealth: 100f)
            .Build();

        SetUtilityWithModifiers(utility);
    }

    private void StartMelee(Transform firePoint)
    {
        // Get target location
        Vector3 lookPos = _agent.Metrics.GetDirectionToTarget();
        lookPos.y = 0;

        // Play animation and start effect sequence
        //_animator.Play("MeleeAttack");
        _agent.StartCoroutine(SequencedEffects(lookPos.normalized));
    }

    private IEnumerator SequencedEffects(Vector3 direction)
    {
        if (!IsSurroundingClear(_agent.firePoint, direction))
            yield break;

        // Calculate impact position
        Vector3 impactPos = _agent.transform.position + direction * 4f;
        // Create spawn position slightly above ground
        Vector3 spawnPosition = _agent.transform.position;
        spawnPosition.y = 0.001f;

        // Spawn warning indicator
        GameObject obj = Instantiate(
            warningIndicator,
            spawnPosition,
            Helpers.GetYAxisLookRotation(direction)
        );

        _agent.GetModule<NavMeshAgentModule>()?.PauseFor(1f);
        _agent.GetModule<Brain>().PauseFor(1f);
        obj.GetComponentInChildren<WarningIndicator>().Initialize(_agent);
        // Wait for animation to reach impact frame
        yield return new WaitForSeconds(1f);

        // Spawn impact particles at impact position
        GameObject impactParticles = Instantiate(
            impactParticlePrefab,
            impactPos,
            Quaternion.identity
        );
        Destroy(impactParticles, 2f);

        // Start explosion sequence from impact position
        int explosionCount = 3;
        float explosionSpacing = 6f;

        for (int i = 0; i < explosionCount; i++)
        {
            Vector3 spawnPos = impactPos + direction * (i * explosionSpacing);
            GameObject explosion = Instantiate(
                explosionPrefab,
                spawnPos,
                Quaternion.LookRotation(direction)
            );
            Destroy(explosion, 2f);

            // // Debug sphere visualization
            // GameObject debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // debugSphere.transform.position = spawnPos;
            // debugSphere.transform.localScale = Vector3.one * explosionRadius * 2f; // *2 because radius is half the scale
            // var renderer = debugSphere.GetComponent<Renderer>();
            // renderer.material.color = new Color(1f, 0f, 0f, 0.3f); // Semi-transparent red
            // Destroy(debugSphere.GetComponent<Collider>()); // Remove collider
            // Destroy(debugSphere, 2f);

            // Add explosion damage
            Collider[] hitColliders = Physics.OverlapSphere(spawnPos, explosionRadius, _targetMask);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.transform.TryGetComponent<Agent>(out var agent))
                {
                    agent.GetModule<HealthModule>().TakeDamage(100);
                }
            }

            yield return new WaitForSeconds(0.1f);
        }

        // Wait for full animation to complete
        yield return new WaitForSeconds(2f - 0.35f - (explosionCount * 0.1f));
    }
}
