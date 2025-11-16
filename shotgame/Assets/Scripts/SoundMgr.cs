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

    public List<AudioClip> soundClips;
    
    [Header("Background Music")]
    private AudioSource bgmAudioSource;
    private bool isBGMPlaying = false;

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
        
        // Create AudioSource for background music
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        bgmAudioSource.loop = true;
        bgmAudioSource.playOnAwake = false;
    }

    void OnEnable()
    {
        // Subscribe to game start event
        UIManager.GameStart += OnGameStart;
        
        // Subscribe to player death event
        Health.PlayerDead += OnPlayerDead;
    }

    void OnDisable()
    {
        // Unsubscribe from game start event
        UIManager.GameStart -= OnGameStart;
        
        // Unsubscribe from player death event
        Health.PlayerDead -= OnPlayerDead;
    }

    void OnGameStart()
    {
        if (isBGMPlaying) return; // Prevent playing multiple times
        
        PlayBackgroundMusic();
    }

    void OnPlayerDead()
    {
        Debug.Log("[SoundMgr] Player died! Stopping background music.");
        StopBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (soundClips == null || soundClips.Count == 0)
        {
            Debug.LogWarning("[SoundMgr] No sound clips assigned! Cannot play background music.");
            return;
        }

        AudioClip bgmClip = soundClips[0];
        
        if (bgmClip == null)
        {
            Debug.LogWarning("[SoundMgr] First sound clip (background music) is null!");
            return;
        }

        if (bgmAudioSource == null)
        {
            Debug.LogError("[SoundMgr] Background music AudioSource is null!");
            return;
        }

        bgmAudioSource.clip = bgmClip;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
        isBGMPlaying = true;

        Debug.Log($"[SoundMgr] Playing background music: {bgmClip.name}");
    }

    public void StopBackgroundMusic()
    {
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
            isBGMPlaying = false;
            Debug.Log("[SoundMgr] Stopped background music.");
        }
    }

    public void SetBGMVolume(float volume)
    {
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = Mathf.Clamp01(volume);
        }
    }

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

    private void OnDestroy()
    {
        // Clean up singleton reference when destroyed
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
