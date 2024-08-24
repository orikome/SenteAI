using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerMovement))]
public class Player : MonoBehaviour, IDamageable
{
    public float MaxHealth => 20000f;
    public float CurrentHealth => 20000f;
    private float currentHealth;
    public static Player Instance { get; private set; }
    private PlayerMovement playerMovement;
    public PlayerMetrics PlayerMetrics { get; private set; }

    void Awake()
    {
        Instance = this;
        currentHealth = MaxHealth;
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        PlayerMetrics = gameObject.GetComponent<PlayerMetrics>();
    }

    public void TakeDamage(int amount)
    {
        PlayerMetrics.damageTaken += amount;
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
