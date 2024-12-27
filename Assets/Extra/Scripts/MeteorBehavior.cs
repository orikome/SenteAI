using UnityEngine;

public class MeteorBehavior : MonoBehaviour
{
    public float fallSpeed = 30f;
    public float explosionRadius = 12f;
    public int explosionDamage = 100;
    public GameObject explosionPrefab;
    private LayerMask _targetMask;
    private LayerMask _ownerMask;
    Agent _agent;

    public void Initialize(Agent agent)
    {
        _agent = agent;

        if (_agent == null || agent.Target == null)
            return;

        Faction faction = agent.Faction;
        Transform target = agent.Target.transform;

        if (faction == Faction.Player || faction == Faction.Ally)
        {
            target = agent.Target.transform;
            _targetMask = LayerMask.GetMask("Enemy");
            if (faction == Faction.Player)
                _ownerMask = LayerMask.GetMask("Player");
            else
                _ownerMask = LayerMask.GetMask("Ally");
            int projectileLayer = LayerMask.NameToLayer("PlayerProjectile");
            gameObject.layer = projectileLayer;
        }
        else
        {
            target = agent.Target.transform;
            _targetMask = LayerMask.GetMask("Player", "Ally");
            _ownerMask = LayerMask.GetMask("Enemy");
            int projectileLayer = LayerMask.NameToLayer("EnemyProjectile");
            gameObject.layer = projectileLayer;
        }
    }

    void Update()
    {
        // Move downward
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        //rb.AddForce(Vector3.down * fallSpeed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Spawn explosion effects
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(
                explosionPrefab,
                transform.position,
                Quaternion.identity
            );
            Destroy(explosion, 2f);
        }

        // Deal explosion damage
        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position,
            explosionRadius,
            _targetMask
        );

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform.root.TryGetComponent<Agent>(out var agent))
            {
                agent.GetModule<HealthModule>().TakeDamage(explosionDamage);
            }
        }

        // Optional: Visualize explosion radius in debug mode
        Debug.DrawRay(transform.position, Vector3.up * explosionRadius, Color.red, 1f);
        Debug.DrawRay(transform.position, Vector3.down * explosionRadius, Color.red, 1f);
        Debug.DrawRay(transform.position, Vector3.left * explosionRadius, Color.red, 1f);
        Debug.DrawRay(transform.position, Vector3.right * explosionRadius, Color.red, 1f);

        // Destroy the meteor
        Destroy(gameObject);
    }
}
