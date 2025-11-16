
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z; // Keep original depth
        Vector3 mouseCamPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        
        transform.position = mouseCamPos;
    }
}
