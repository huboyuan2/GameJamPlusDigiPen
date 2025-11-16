using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVShaderController : MonoBehaviour
{
    public Renderer rend;
    private Material material;

    void OnEnable()
    {
        // Subscribe to GameStart event
        UIManager.GameStart += OnGameStart;
    }

    void OnDisable()
    {
        // Unsubscribe from GameStart event
        UIManager.GameStart -= OnGameStart;
    }

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();

        if (rend != null)
        {
            // Get the material instance
            material = rend.material;

            // Set _uvspeed to 0 at start
            if (material.HasProperty("_uvspeed"))
            {
                material.SetFloat("_uvspeed", 0f);
                Debug.Log("[UVShaderController] Set _uvspeed to 0 at Start");
            }
            else
            {
                Debug.LogWarning("[UVShaderController] Material does not have _uvspeed property!");
            }
        }
        else
        {
            Debug.LogError("[UVShaderController] Renderer component not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Called when game starts
    private void OnGameStart()
    {
        if (material != null && material.HasProperty("_uvspeed"))
        {
            material.SetFloat("_uvspeed", 0.25f);
            Debug.Log("[UVShaderController] Set _uvspeed to 0.25 on GameStart");
        }
    }

    void OnDestroy()
    {
        // Clean up material instance to prevent memory leaks
        if (material != null)
        {
            Destroy(material);
        }
    }
}