using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadTimer : MonoBehaviour
{
    public bool isActive = false;

    public float duration;

    private bool once = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive)
        {
            return;
        }

        if (!once)
        {
            Destroy(this.gameObject, duration);
            once = true;
        }
    }
}
