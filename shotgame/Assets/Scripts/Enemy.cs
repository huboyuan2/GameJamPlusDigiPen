using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // This is not cool, in real project we should use ScriptableObject/json/csv for enemy stats, but for gamejam it's ok
    public int maxHealth = 30;
    private int health;
    private DummyEnemy dummyEnemy;

    void Start()
    {
        health = maxHealth;
        dummyEnemy = GetComponent<DummyEnemy>();
    }

    void Update()
    {

    }

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

    void HandlePlayerCollision(Collider2D collision)
    {
        // Handle player collision
        Debug.Log("Player touched enemy!");
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

        if (dummyEnemy != null)
        {
            dummyEnemy.Die();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}