using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : MonoBehaviour
{
    // Singleton instance
    private static SoundMgr _instance;

    public static SoundMgr Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SoundMgr>();
                
                // If still not found, create a new GameObject with SoundMgr
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("SoundMgr");
                    _instance = singletonObject.AddComponent<SoundMgr>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // Enforce singleton pattern
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"[SoundMgr] Duplicate instance detected on {gameObject.name}. Destroying...");
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public List<AudioClip> soundClips;
    public void PlaySound(int soundIndex, Vector3 position)
    {
        if (soundIndex < 0 || soundIndex >= soundClips.Count)
        {
            Debug.LogWarning($"Sound index '{soundIndex}' is out of range.");
            return;
        }
        AudioClip soundClip = soundClips[soundIndex];
        if (soundClip != null)
        {
            AudioSource.PlayClipAtPoint(soundClip, position);
        }
        else
        {
            Debug.LogWarning($"Sound '{soundIndex}' not found in SoundMgr.");
        }
    }
    //void Start()
    //{

    //}

    //void Update()
    //{

    //}

    private void OnDestroy()
    {
        // Clean up singleton reference when destroyed
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
