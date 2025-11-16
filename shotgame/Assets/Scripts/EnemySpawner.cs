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

    [Header("State")]
    private bool isGameStarted = false;
    private bool isPlayerDead = false;

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

    void OnEnable()
    {
        // Subscribe to game start event
        UIManager.GameStart += OnGameStart;
        
        // Subscribe to player death event
        Health.PlayerDead += OnPlayerDead;
    }

    void OnDisable()
    {
        // Unsubscribe from game start event
        UIManager.GameStart -= OnGameStart;
        
        // Unsubscribe from player death event
        Health.PlayerDead -= OnPlayerDead;
    }

    // Update is called once per frame
    void Update()
    {
        // Don't spawn if game hasn't started yet
        if (!isGameStarted) return;
        
        if (isPlayerDead) return; // Stop spawning if player is dead

        if (activeEnemies.Count < 5)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        // Don't spawn if game hasn't started yet
        if (!isGameStarted) return;
        
        if (isPlayerDead) return; // Safety check

        if (enemyTemplates.Count == 0 || spawnPoints.Count == 0)
        {
            Debug.LogWarning("[EnemySpawner] No enemy templates or spawn points assigned!");
            return;
        }

        // Choose a random enemy template and spawn point
        GameObject enemyPrefab = enemyTemplates[Random.Range(0, enemyTemplates.Count)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        // Instantiate the enemy and add it to the active list
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        activeEnemies.Add(enemy);
    }

    public void TelePortEnemy(Enemy enemy)
    {
        if (isPlayerDead) return; // Don't teleport if player is dead

        if (activeEnemies.Contains(enemy.gameObject) && spawnPoints.Count > 0)
        {
            enemy.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        }
    }

    void OnGameStart()
    {
        if (isGameStarted) return; // Prevent multiple calls
        isGameStarted = true;
        
        Debug.Log("[EnemySpawner] Game started! Beginning enemy spawning.");
    }

    void OnPlayerDead()
    {
        if (isPlayerDead) return; // Prevent multiple calls
        isPlayerDead = true;

        Debug.Log("[EnemySpawner] Player died! Stopping enemy spawning. Enemies will continue moving but stop shooting.");
        
        // Note: We don't call StopAllEnemies() anymore
        // Each Enemy script will handle player death themselves via Health.PlayerDead event
        // Enemies will continue moving but stop shooting and teleporting
    }
}