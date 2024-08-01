using UnityEngine;

public class PlayerMetrics : MonoBehaviour
{
    public enum PlayerBehavior
    {
        Aggressive,
        Defensive,
        Balanced
    }

    [Header("Player Metrics")]
    public float shootingFrequency;
    public float dodgeRatio;
    public float averageDistanceFromBoss;
    public float movementSpeed;

    [Header("Behavior Thresholds")]
    [SerializeField]
    private float aggressiveThreshold = 0.7f;

    [SerializeField]
    private float defensiveThreshold = 0.3f;

    public PlayerBehavior currentBehavior;

    void Update()
    {
        UpdatePlayerMetrics();
        currentBehavior = ClassifyBehavior();
        //RespondToPlayerBehavior();
    }

    void UpdatePlayerMetrics()
    {
        shootingFrequency = Random.Range(0f, 1f);
        dodgeRatio = Random.Range(0f, 1f);
        averageDistanceFromBoss = Random.Range(0f, 1f);
        movementSpeed = Random.Range(0f, 1f);
    }

    PlayerBehavior ClassifyBehavior()
    {
        if (shootingFrequency > aggressiveThreshold && averageDistanceFromBoss < defensiveThreshold)
            return PlayerBehavior.Aggressive;

        if (dodgeRatio > aggressiveThreshold)
            return PlayerBehavior.Defensive;

        return PlayerBehavior.Balanced;
    }

    void RespondToPlayerBehavior()
    {
        switch (currentBehavior)
        {
            case PlayerBehavior.Aggressive:
                Debug.Log("Player is Aggressive");
                break;
            case PlayerBehavior.Defensive:
                Debug.Log("Player is Defensive");
                break;
            case PlayerBehavior.Balanced:
                Debug.Log("Player is Balanced");
                break;
        }
    }
}
