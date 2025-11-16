using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private static EnemySpawner _instance;

    public static EnemySpawner Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EnemySpawner>();

                if (_instance == null)
                {
                    Debug.LogWarning("[EnemySpawner] No EnemySpawner found in scene!");
                }
            }
            return _instance;
        }
    }

    public List<GameObject> enemyTemplates;
    public List<Transform> spawnPoints;
    public List<GameObject> activeEnemies;

    void Awake()
    {
        // Singleton pattern
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning("[EnemySpawner] Multiple EnemySpawner instances detected. Destroying duplicate.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (activeEnemies.Count < 5)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        // Choose a random enemy template and spawn point
        GameObject enemyPrefab = enemyTemplates[Random.Range(0, enemyTemplates.Count)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        // Instantiate the enemy and add it to the active list
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        activeEnemies.Add(enemy);
    }

    public void TelePortEnemy(Enemy enemy)
    {
        if (activeEnemies.Contains(enemy.gameObject))
        {
            enemy.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        }
    }
}