using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;
    Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        //rb.velocity = transform.up * speed;

        Destroy(gameObject, lifeTime);  
    }

    public void SetDirection(Vector2 dirvector)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.velocity = dirvector.normalized * speed;
        float angle = Mathf.Atan2(dirvector.y, dirvector.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
