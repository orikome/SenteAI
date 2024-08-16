using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerMovement))]
public class Player : MonoBehaviour, IDamageable
{
    public float MaxHealth => 20000f;
    public float CurrentHealth => 20000f;
    private float currentHealth;
    public static Player Instance;
    private PlayerMovement playerMovement;
    private PlayerMetrics playerMetrics;

    void Awake()
    {
        Instance = this;
        currentHealth = MaxHealth;
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerMetrics = gameObject.GetComponent<PlayerMetrics>();
    }

    public void TakeDamage(int amount)
    {
        playerMetrics.damageTaken += amount;
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
