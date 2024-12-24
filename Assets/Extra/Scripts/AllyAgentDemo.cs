using UnityEngine;

public class AllyAgentDemo : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private GameObject allyPrefab;
    private float spawnTimer = 0f;
    private const float SPAWN_INTERVAL = 5f;

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= SPAWN_INTERVAL)
        {
            SpawnEnemyAndAlly();
            spawnTimer = 0f;
        }
    }

    private void SpawnEnemyAndAlly()
    {
        if (enemyPrefab != null)
        {
            GameObject enemyObject = Instantiate(
                enemyPrefab,
                OrikomeUtils.GeneralUtils.GetRandomPositionInCircle(Vector3.zero, 10f),
                Quaternion.identity
            );
            Agent enemyAgent = enemyObject.GetComponent<Agent>();
            enemyAgent.Initialize();

            GameObject allyObject = Instantiate(
                allyPrefab,
                OrikomeUtils.GeneralUtils.GetRandomPositionInCircle(Vector3.zero, 10f),
                Quaternion.identity
            );
            Agent allyAgent = allyObject.GetComponent<Agent>();
            allyAgent.Initialize();
        }
    }
}
