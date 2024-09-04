using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerMovement))]
public class Player : MonoBehaviour, IDamageable
{
    public static Player Instance { get; private set; }
    public PlayerMetrics Metrics { get; private set; }
    public float MaxHealth => 100f;
    public float CurrentHealth => 100f;
    private float _currentHealth;
    private PlayerMovement _playerMovement;
    public bool IsAlive { get; set; }

    void Awake()
    {
        Instance = this;
        _currentHealth = MaxHealth;
        _playerMovement = gameObject.GetComponent<PlayerMovement>();
        Metrics = gameObject.GetComponent<PlayerMetrics>();
        IsAlive = true;
    }

    public void TakeDamage(int amount)
    {
        Metrics.damageTaken += amount;
        _currentHealth -= amount;
        _currentHealth = Mathf.Max(_currentHealth, 0);

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        IsAlive = false;
        GameManager.Instance.RestartScene();
        //Destroy(gameObject);
    }
}
