using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHp;
    private float hp = 0;
    [SerializeField] private HealthBar healthBar;

    [Header("Damage Effect Settings")]
    public Color damageColor = Color.red;
    public float damageEffectDuration = 0.5f;

    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    private Dictionary<SpriteRenderer, Color> originalColors = new Dictionary<SpriteRenderer, Color>();
    private bool isPlayingDamageEffect = false;

    // Start is called before the first frame update
    void Start()
    {
        hp = maxHp;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (healthBar != null)
        {
            healthBar.SetHealthBar(hp);
        }
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;

        // Play damage effect
        DamageEffect();

        // Check for death
        if (hp <= 0)
        {
            Die();
        }
    }

    void DamageEffect()
    {
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
        // Add death logic here (e.g., trigger animation, destroy object, etc.)
    }
}