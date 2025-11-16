using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMgr : MonoBehaviour
{
    // Singleton instance
    private static EffectMgr _instance;

    public static EffectMgr Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EffectMgr>();
                
                // If still not found, create a new GameObject with EffectMgr
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("EffectMgr");
                    _instance = singletonObject.AddComponent<EffectMgr>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }
    public List<GameObject> effectPrefabs;

    public void PlayEffect(int effectIndex, Vector3 position)
    {
        if (effectIndex < 0 || effectIndex >= effectPrefabs.Count)
        {
            Debug.LogWarning($"Effect index '{effectIndex}' is out of range.");
            return;
        }

        GameObject effectPrefab = effectPrefabs[effectIndex];
        if (effectPrefab != null)
        {
            Instantiate(effectPrefab, position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning($"Effect '{effectIndex}' not found in EffectMgr.");
        }
    }

    private void Awake()
    {
        // Enforce singleton pattern
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"[EffectMgr] Duplicate instance detected on {gameObject.name}. Destroying...");
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        // Clean up singleton reference when destroyed
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
