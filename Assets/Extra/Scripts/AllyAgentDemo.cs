using UnityEngine;

public class AllyAgentDemo : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private GameObject allyPrefab;

    [SerializeField]
    private GameObject enemyPrefabT2;

    [SerializeField]
    private GameObject allyPrefabT2;
    private float spawnTimer = 0f;
    private const float SPAWN_INTERVAL = 2.5f;

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= SPAWN_INTERVAL)
        {
            int enemyCount = AgentManager.Instance.activeEnemies.Count;
            int allyCount = AgentManager.Instance.activeAllies.Count;

            // Spawn whichever has fewer
            if (enemyCount < allyCount)
            {
                SpawnEnemy();
                SpawnEnemy();
            }
            else if (allyCount < enemyCount)
            {
                SpawnAlly();
                SpawnAlly();
            }
            else
            {
                if (Random.value < 0.5f)
                {
                    SpawnEnemy();
                    SpawnEnemy();
                }
                else
                {
                    SpawnAlly();
                    SpawnAlly();
                }
            }

            spawnTimer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab != null)
        {
            GameObject prefabToSpawn = Random.value < 0.7f ? enemyPrefab : enemyPrefabT2;
            if (prefabToSpawn != null)
            {
                GameObject enemyObject = Instantiate(
                    prefabToSpawn,
                    OrikomeUtils.GeneralUtils.GetRandomPositionInCircle(Vector3.zero, 60f),
                    Quaternion.identity
                );
                Agent enemyAgent = enemyObject.GetComponent<Agent>();
                enemyAgent.Initialize();
            }
        }
    }

    private void SpawnAlly()
    {
        if (allyPrefab != null)
        {
            GameObject prefabToSpawn = Random.value < 0.7f ? allyPrefab : allyPrefabT2;
            if (prefabToSpawn != null)
            {
                GameObject allyObject = Instantiate(
                    prefabToSpawn,
                    OrikomeUtils.GeneralUtils.GetRandomPositionInCircle(Vector3.zero, 60f),
                    Quaternion.identity
                );
                Agent allyAgent = allyObject.GetComponent<Agent>();
                allyAgent.Initialize();
            }
        }
    }
}
