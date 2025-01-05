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

    [SerializeField]
    private GameObject enemyBoss;
    private float spawnTimer = 0f;
    private const float SPAWN_INTERVAL = 2.5f;
    private const float BOSS_SPAWN_INTERVAL = 60f;
    private float bossTime = 0f;

    private void Update()
    {
        bossTime += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        if (bossTime >= BOSS_SPAWN_INTERVAL)
        {
            if (enemyBoss != null)
            {
                GameObject bossObject = Instantiate(
                    enemyBoss,
                    OrikomeUtils.GeneralUtils.GetRandomPositionInCircle(Vector3.zero, 60f),
                    Quaternion.identity
                );
                Agent bossAgent = bossObject.GetComponent<Agent>();
                bossAgent.Initialize();
                bossTime = 0f;
            }
        }

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
