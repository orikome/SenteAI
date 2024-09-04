using OrikomeUtils;
using UnityEngine;

public class HomingOrbBehaviour : MonoBehaviour
{
    public float speed = 5f;
    public float homingIntensity = 0.5f;
    private Transform player;
    private Rigidbody rb;
    public LayerMask playerMask;

    void Start()
    {
        player = Player.Instance.transform;
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 12f);
    }

    void Update()
    {
        HomeTowardsPlayer();
    }

    void HomeTowardsPlayer()
    {
        if (player == null)
            return;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        rb.velocity = Vector3.Lerp(rb.velocity, directionToPlayer * speed, homingIntensity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(10);
            Helpers.SpawnParticles(transform.position, Color.red);
            Debug.Log(
                $"{Helpers.CleanName(gameObject.name)} dealt {10} damage to {Helpers.CleanName(collision.gameObject.name)}"
            );
            Destroy(gameObject);
            return;
        }

        Helpers.SpawnParticles(transform.position, Color.red);
        Destroy(gameObject);
    }
}
