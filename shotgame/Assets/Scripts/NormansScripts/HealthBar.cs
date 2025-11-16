using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider _hpBar;
    
    public Transform player; // player
    public Vector3 offset;   // (0, someHeight, 0)
    private Camera cam;
    
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;

        // Ensure slider is configured correctly
        if (_hpBar != null)
        {
            _hpBar.minValue = 0f;
            _hpBar.maxValue = 1f; // Normalized range (0-1)
            _hpBar.value = 1f;    // Start at full health
        }
    }
    
    void LateUpdate()
    {
        if (player != null && cam != null)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(player.position + offset);
            transform.position = screenPos;
        }
    }

    // Initialize health bar with maxHp (called from Health.Start())
    public void Initialize(float maxHp)
    {
        if (_hpBar != null)
        {
            _hpBar.minValue = 0f;
            _hpBar.maxValue = 1f; // Use normalized range
            _hpBar.value = 1f;    // Start at full
        }

        Debug.Log($"[HealthBar] Initialized with max HP: {maxHp}");
    }

    // Set health bar value (expects normalized value 0-1)
    public void SetHealthBar(float normalizedHealth)
    {
        if (_hpBar != null)
        {
            _hpBar.value = Mathf.Clamp01(normalizedHealth); // Ensure 0-1 range
        }
    }
}
