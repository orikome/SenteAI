using UnityEngine;

public class Player : Agent, IDamageable
{
    public static Player Instance { get; private set; }
    public PlayerMetrics Metrics { get; private set; }
    public bool IsAlive { get; private set; }
    public float MaxHealth => 1000f;
    public float CurrentHealth => 1000f;
    private float _currentHealth;

    void Awake()
    {
        Instance = this;
        _currentHealth = MaxHealth;
        Metrics = gameObject.GetComponent<PlayerMetrics>();
        IsAlive = true;
        LoadAgentData();
        InitModules();
    }

    public override void Update()
    {
        foreach (var module in Modules)
        {
            module.Execute(null);
        }
    }

    public void TakeDamage(int amount)
    {
        Metrics.UpdateDamageDone(amount);
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
