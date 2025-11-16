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
    private SpriteRenderer spriteRenderer;
    private float frameInterval;
    private float timer;
    private int currentFrame = 0;
    private bool isPlaying = false;

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

        if (playOnStart)
        {
            Play();
        }
    }

    void Update()
    {
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

    #region Public Methods

    // Start playing animation
    public void Play()
    {
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