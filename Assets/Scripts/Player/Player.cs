using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerMovement))]
public class Player : MonoBehaviour, IDamageable
{
    public static Player Instance { get; private set; }
    public PlayerMetrics PlayerMetrics { get; private set; }
    public float MaxHealth => 100f;
    public float CurrentHealth => 100f;
    private float _currentHealth;
    private PlayerMovement _playerMovement;

    void Awake()
    {
        Instance = this;
        _currentHealth = MaxHealth;
        _playerMovement = gameObject.GetComponent<PlayerMovement>();
        PlayerMetrics = gameObject.GetComponent<PlayerMetrics>();
    }

    public void TakeDamage(int amount)
    {
        PlayerMetrics.damageTaken += amount;
        _currentHealth -= amount;
        _currentHealth = Mathf.Max(_currentHealth, 0);

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
