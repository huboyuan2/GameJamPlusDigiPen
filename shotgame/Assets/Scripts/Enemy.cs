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
    private EnemySpawner spawner;
    void Start()
    {
        health = maxHealth;
        dummyEnemy = GetComponent<DummyEnemy>();
        spawner = FindObjectOfType<EnemySpawner>();
    }

    void Update()
    {
        // Update speed based on Y position
        globalSpeed = CalculateGlobalSpeed();

        // Apply movement (assuming moving left)
        transform.position += Vector3.left * globalSpeed * Time.deltaTime;
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
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Use switch pattern for cleaner code
    //    switch (collision.collider.tag)
    //        {
    //            case "Player":
    //                HandlePlayerCollision(collision.collider);
    //                break;
    //            case "Bullet":
    //                HandleBulletCollision(collision.collider);
    //                break;
    //        }
    //}
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
}