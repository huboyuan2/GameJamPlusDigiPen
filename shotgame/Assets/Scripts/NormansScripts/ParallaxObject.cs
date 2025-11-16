using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxObject : MonoBehaviour
{
    [SerializeField] float speed = 0.5f;
    public bool isMoving = true;
    void Update()
    {
        if (!isMoving)
        {
            return;
        }
        transform.position += Vector3.left * (speed * Time.deltaTime);
        
    }
}
