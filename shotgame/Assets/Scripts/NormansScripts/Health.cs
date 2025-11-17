using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHp;
    [SerializeField] private HealthBar healthBar;

    [Header("Damage Effect Settings")]
    public Color damageColor = Color.red;
    public float damageEffectDuration = 0.5f;

    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    private Dictionary<SpriteRenderer, Color> originalColors = new Dictionary<SpriteRenderer, Color>();
    private bool isPlayingDamageEffect = false;
    
    public static Action PlayerDead;

    // Private backing field for HP
    private float _hp = 0;

    // Public property with change detection
    public float Hp
    {
        get => _hp;
        private set
        {
            // Only update if value actually changed
            if (Mathf.Approximately(_hp, value)) return;

            _hp = Mathf.Clamp(value, 0, maxHp); // Clamp to valid range

            // Update health bar when HP changes
            UpdateHealthBar();

            // Check for death
            if (_hp <= 0 && !isDead)
            {
                isDead = true;
                Die();
            }
        }
    }

    // Public read-only property for maxHp
    public float MaxHp => maxHp;

    // Public read-only property for normalized HP (0-1)
    public float NormalizedHp => maxHp > 0 ? _hp / maxHp : 0;

    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize HP (this will trigger health bar update)
        Hp = maxHp;

        // Cache all SpriteRenderers in children, excluding "Shadow"
        SpriteRenderer[] allSpriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        foreach (var sr in allSpriteRenderers)
        {
            if (sr != null && sr.gameObject.name != "Shadow")
            {
                spriteRenderers.Add(sr);
                originalColors[sr] = sr.color; // Store original color
            }
        }

        Debug.Log($"[Health] Cached {spriteRenderers.Count} SpriteRenderers (excluding Shadow)");

        // Initialize health bar with maxHp
        if (healthBar != null)
        {
            healthBar.Initialize(maxHp);
        }
    }

    // No longer need Update() - health bar updates only when HP changes
    // void Update() removed

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Don't take damage if already dead

        // Use property setter (will automatically update health bar)
        Hp -= damage;

        Debug.Log($"[Health] {gameObject.name} took {damage} damage. Current HP: {Hp}/{MaxHp}");

        // Play damage effect
        DamageEffect();
    }

    public void Heal(float amount)
    {
        if (isDead) return; // Can't heal if dead

        Hp += amount;

        Debug.Log($"[Health] {gameObject.name} healed {amount}. Current HP: {Hp}/{MaxHp}");
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.SetHealthBar(NormalizedHp); // Pass normalized value (0-1)
        }
    }

    void DamageEffect()
    {
        SoundMgr.Instance.PlaySound(4, transform.position);
        if (isPlayingDamageEffect) return; // Prevent overlapping effects

        isPlayingDamageEffect = true;

        // Change all sprites to damage color (Shadow already excluded)
        foreach (var sr in spriteRenderers)
        {
            if (sr != null)
            {
                sr.DOColor(damageColor, 0.1f); // Quick transition to red
            }
        }

        // Restore original colors after duration
        DOVirtual.DelayedCall(damageEffectDuration, () =>
        {
            foreach (var sr in spriteRenderers)
            {
                if (sr != null && originalColors.ContainsKey(sr))
                {
                    sr.DOColor(originalColors[sr], 0.1f); // Quick transition back
                }
            }
            isPlayingDamageEffect = false;
        });
    }

    void Die()
    {
        Debug.Log($"[Health] {gameObject.name} died!");
        SoundMgr.Instance.PlaySound(5, transform.position);
        // Hide health bar on death
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        // Invoke death event
        PlayerDead?.Invoke();
    }
}