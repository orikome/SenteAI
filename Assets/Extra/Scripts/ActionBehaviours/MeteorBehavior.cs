using UnityEngine;

public class MeteorBehavior : ActionBehaviour
{
    public float fallSpeed = 30f;
    public float explosionRadius = 8f;
    public int explosionDamage = 100;
    public GameObject explosionPrefab;

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
            if (hitCollider.gameObject.TryGetComponent<Agent>(out var agent))
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
