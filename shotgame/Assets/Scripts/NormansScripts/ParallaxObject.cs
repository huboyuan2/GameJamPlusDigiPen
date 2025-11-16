using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxObject : MonoBehaviour
{
    [SerializeField] float speed = 0.5f;

    void Update()
    {
        transform.position += Vector3.left * (speed * Time.deltaTime);
    }
}
