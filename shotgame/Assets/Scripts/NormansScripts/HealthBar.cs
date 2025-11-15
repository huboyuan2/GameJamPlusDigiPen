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
    }
    
    void LateUpdate()
    {
        Vector3 screenPos = cam.WorldToScreenPoint(player.position + offset);
        transform.position = screenPos;
    }

    public void SetHealthBar(float health)
    {
        _hpBar.value = health;
    }
}
