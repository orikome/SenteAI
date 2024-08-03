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
        if (LayerMaskUtils.IsLayerInMask(collision.gameObject.layer, playerMask))
        {
            Destroy(gameObject);
        }
    }
}
