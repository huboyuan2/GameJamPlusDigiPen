using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // This is not cool, in real project we should use ScriptableObject/json/csv for enemy stats, but for gamejam it's ok
    public int maxHealth = 30;
    public int damage = 10;
    private int health;
    private DummyEnemy dummyEnemy;

    [Header("Movement Settings")]
    public float speedratio = 1f;                    // Base speed multiplier
    public float yPositionSpeedFactor = 0.1f;        // How much Y position affects speed
    public float referenceY = 0f;                    // Reference Y position (scale = 1.0)
    
    private float globalSpeed;
    
    [Header("Shooting Settings")]
    public GameObject enemyBullet;
    public bool canShoot = true;                     // Can this enemy shoot
    public bool canPredictPlayer = false;            // Predict player movement
    public float fireCooldown = 2f;                  // Time between shots
    public float detectionRange = 150f;               // How far enemy can detect player
    public float bulletSpeed = 10f;                  // Enemy bullet speed
    public float predictionMultiplier = 0.5f;        // How much to lead the target
    

    
    private Transform player;
    private float fireTimer = 0f;
    private EnemySpawner spawner;
    
    void Start()
    {
        health = maxHealth;
        dummyEnemy = GetComponent<DummyEnemy>();
        spawner = FindObjectOfType<EnemySpawner>();
        
        // Find player by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            // Fallback: find by component
            PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
            if (playerMovement != null)
            {
                player = playerMovement.transform;
            }
            else
            {
                Debug.LogWarning("[Enemy] Player not found! Enemy won't shoot.");
            }
        }
        
        // Random initial fire delay to avoid synchronized shooting
        fireTimer = Random.Range(0f, fireCooldown);
    }

    void Update()
    {
        // Update speed based on Y position
        globalSpeed = CalculateGlobalSpeed();

        // Apply movement (assuming moving left)
        transform.position += Vector3.left * globalSpeed * Time.deltaTime;
        
        // Shooting logic
        if (canShoot && player != null && enemyBullet != null)
        {
            fireTimer -= Time.deltaTime;
            
            if (fireTimer <= 0f && IsPlayerInRange())
            {
                Shoot();
                fireTimer = fireCooldown;
            }
        }
        
        if (IsOutOfBounds())
        {
            TelePort();
        }
    }

    float CalculateGlobalSpeed()
    {
        Environment env = FindObjectOfType<Environment>();
        if (env == null) return 0f;

        float baseSpeed = env.scrollSpeed * speedratio;

        // Adjust speed based on Y position
        float yOffset = transform.position.y - referenceY;
        float speedMultiplier = 1.0f - (yOffset * yPositionSpeedFactor);
        speedMultiplier = Mathf.Clamp(speedMultiplier, 0.3f, 1.5f); // Limit range

        globalSpeed = baseSpeed * speedMultiplier;

        return globalSpeed;
    }
    
    #region Shooting Logic
    
    bool IsPlayerInRange()
    {
        if (player == null) return false;
        
        float distance = Vector2.Distance(transform.position, player.position);
        return distance <= detectionRange;
    }
    
    void Shoot()
    {
        
        if (player == null || enemyBullet == null) return;
        Debug.Log("[Enemy] Shooting at player...");
        // Calculate shoot direction
        Vector2 shootDirection;
        
        if (canPredictPlayer)
        {
            // Predict player movement
            shootDirection = CalculatePredictiveDirection();
        }
        else
        {
            // Direct aim at player
            shootDirection = (player.position - transform.position).normalized;
        }
        
        // Spawn bullet
        GameObject bullet = Instantiate(enemyBullet, transform.position, Quaternion.identity);
        
        // Set bullet direction (assuming Bullet script)
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            Debug.Log("[Enemy] Shooting bullet...");
            bulletScript.SetDirection(shootDirection);
        }
        else
        {
            // Fallback: directly set velocity if no Bullet script
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = shootDirection * bulletSpeed;
                
                // Rotate bullet to face direction
                float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
                bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
        
        Debug.Log($"[Enemy] Fired at player! Direction: {shootDirection}");
    }
    
    Vector2 CalculatePredictiveDirection()
    {
        if (player == null) return Vector2.left;
        
        // Get player movement component
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        
        Vector2 playerVelocity = Vector2.zero;
        
        if (playerRb != null)
        {
            playerVelocity = playerRb.velocity;
        }
        
        // Calculate time to reach player
        float distance = Vector2.Distance(transform.position, player.position);
        float timeToReach = distance / bulletSpeed;
        
        // Predict future player position
        Vector2 predictedPosition = (Vector2)player.position + (playerVelocity * timeToReach * predictionMultiplier);
        
        // Calculate direction to predicted position
        Vector2 direction = (predictedPosition - (Vector2)transform.position).normalized;
        
        return direction;
    }
    
    #endregion
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Use switch pattern for cleaner code
        switch (collision.tag)
        {
            case "Player":
                HandlePlayerCollision(collision);
                break;
            case "Bullet":
                HandleBulletCollision(collision);
                break;
        }
    }
    
    void TelePort()
    {
        if (spawner != null)
        {
            spawner.TelePortEnemy(this);
            // Reset fire timer when teleported
            fireTimer = Random.Range(0f, fireCooldown * 0.5f);
            return;
        }
        else
        {
            transform.position = new Vector3(10, transform.position.y, transform.position.z);
            health = maxHealth;
        }
    }
    
    bool IsOutOfBounds()
    {
        return transform.position.x < -15f;
    }
    
    void HandlePlayerCollision(Collider2D collision)
    {
        // Handle player collision
        Health playerHealth = collision.GetComponent<Health>();
        PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
        if (playerHealth != null && playerMovement != null)
        {
            if (!playerMovement.isJumping)
                playerHealth.TakeDamage(damage); // Deal damage to the player
            TakeDamage(maxHealth); // Enemy dies upon collision
        }
    }

    void HandleBulletCollision(Collider2D collision)
    {
        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet == null) return;
        if(!bullet.isPlayerBullet) return; // Ignore enemy bullets   
        TakeDamage(bullet.damage);
        Destroy(collision.gameObject);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"Enemy took {damage} damage. Health: {health}/{maxHealth}");
      
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died!");

        // Remove from active list first (before any destruction)
        if (EnemySpawner.Instance != null)
        {
            EnemySpawner.Instance.activeEnemies.Remove(this.gameObject);
        }
      
        // Handle death animation/destruction
        if (dummyEnemy != null)
        {
            dummyEnemy.Die(); // This will handle destruction internally
        }
        else
        {
            Destroy(gameObject); // Immediate destruction without animation
        }
    }
    
    #region Gizmos (Editor Visualization)
    
    void OnDrawGizmosSelected()
    {
        if (!canShoot) return;
        
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw line to player if in range
        if (player != null && IsPlayerInRange())
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
    
    #endregion
}