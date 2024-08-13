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
    public float distanceToClosestEnemy;
    public float movementSpeed;
    Vector3 lastPosition = Vector3.zero;

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

        distanceToClosestEnemy = OrikomeUtils.GeneralUtils.GetDistanceSquared(
            FindClosestEnemyToPlayer().position,
            transform.position
        );

        movementSpeed =
            OrikomeUtils.GeneralUtils.GetDistanceSquared(transform.position, lastPosition)
            / Time.deltaTime;
        lastPosition = transform.position;
    }

    public Transform FindClosestEnemyToPlayer()
    {
        Agent closestEnemy = null;
        float closestEnemyDistance = Mathf.Infinity;

        foreach (Agent agent in GameManager.Instance.activeAgents)
        {
            float distance = OrikomeUtils.GeneralUtils.GetDistanceSquared(
                agent.transform.position,
                transform.position
            );

            if (distance < closestEnemyDistance)
            {
                closestEnemyDistance = distance;
                closestEnemy = agent;
            }
        }

        return closestEnemy?.transform;
    }

    PlayerBehavior ClassifyBehavior()
    {
        if (shootingFrequency > aggressiveThreshold && distanceToClosestEnemy < defensiveThreshold)
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
