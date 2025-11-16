using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnimController : MonoBehaviour
{
    [Header("Animation Sprites")]
    public Sprite frame1;
    public Sprite frame2;

    [Header("Animation Settings")]
    public float frameRate = 10f; // Frames per second
    public bool playOnStart = true;
    public bool loop = true;
    
    [Header("State")]
    private bool isGameStarted = false;
    
    private SpriteRenderer spriteRenderer;
    private float frameInterval;
    private float timer;
    private int currentFrame = 0;
    private bool isPlaying = false;

    void OnEnable()
    {
        // Subscribe to game start event
        UIManager.GameStart += OnGameStart;
    }

    void OnDisable()
    {
        // Unsubscribe from game start event
        UIManager.GameStart -= OnGameStart;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("[SimpleAnimController] No SpriteRenderer found on this GameObject!");
            enabled = false;
            return;
        }

        if (frame1 == null || frame2 == null)
        {
            Debug.LogWarning("[SimpleAnimController] Frame sprites not assigned!");
        }

        // Calculate interval between frames
        frameInterval = 1f / frameRate;

        // Note: Don't play on start anymore - wait for game start event
        // Animation will start when OnGameStart() is called
        if (playOnStart)
        {
            // Set to first frame but don't start playing yet
            currentFrame = 0;
            UpdateSprite();
        }
    }

    void Update()
    {
        // Don't play animation if game hasn't started yet
        if (!isGameStarted) return;
        
        if (!isPlaying) return;

        timer += Time.deltaTime;

        if (timer >= frameInterval)
        {
            timer -= frameInterval;
            NextFrame();
        }
    }

    void NextFrame()
    {
        currentFrame++;

        if (currentFrame >= 2)
        {
            if (loop)
            {
                currentFrame = 0;
            }
            else
            {
                currentFrame = 1;
                isPlaying = false;
                return;
            }
        }

        UpdateSprite();
    }

    void UpdateSprite()
    {
        if (spriteRenderer == null) return;

        switch (currentFrame)
        {
            case 0:
                if (frame1 != null) spriteRenderer.sprite = frame1;
                break;
            case 1:
                if (frame2 != null) spriteRenderer.sprite = frame2;
                break;
        }
    }

    void OnGameStart()
    {
        if (isGameStarted) return; // Prevent multiple calls
        isGameStarted = true;

        Debug.Log("[SimpleAnimController] Game started! Beginning animation playback.");

        // Start playing animation if playOnStart is enabled
        if (playOnStart)
        {
            Play();
        }
    }

    #region Public Methods

    // Start playing animation
    public void Play()
    {
        // Only allow playing if game has started
        if (!isGameStarted)
        {
            Debug.LogWarning("[SimpleAnimController] Cannot play animation - game hasn't started yet!");
            return;
        }

        isPlaying = true;
        currentFrame = 0;
        timer = 0f;
        UpdateSprite();
    }

    // Stop animation
    public void Stop()
    {
        isPlaying = false;
    }

    // Pause animation (can resume from current frame)
    public void Pause()
    {
        isPlaying = false;
    }

    // Resume animation
    public void Resume()
    {
        // Only allow resuming if game has started
        if (!isGameStarted)
        {
            Debug.LogWarning("[SimpleAnimController] Cannot resume animation - game hasn't started yet!");
            return;
        }

        isPlaying = true;
    }

    // Set animation speed (frames per second)
    public void SetFrameRate(float fps)
    {
        frameRate = Mathf.Max(1f, fps);
        frameInterval = 1f / frameRate;
    }

    // Reset to first frame
    public void Reset()
    {
        currentFrame = 0;
        timer = 0f;
        UpdateSprite();
    }

    #endregion
}