using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHp;
    private float hp = 0; 
    [SerializeField] private HealthBar healthBar;
     
    // Start is called before the first frame update
    void Start()
    {
        hp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.SetHealthBar(hp);
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
    }
}
