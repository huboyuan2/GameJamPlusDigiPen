using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CamController : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player;
    public float followSmoothTime = 0.3f;
    public Vector3 followOffset = new Vector3(0, 0, -10f);
    public Transform background;
    [Header("Default Position")]
    public Vector3 defaultPosition = new Vector3(0, 0, -10f);
    public float switchDuration = 0.5f;
    public Ease switchEase = Ease.OutCubic;

    [Header("Camera Shake")]
    public float shakeDuration = 0.3f;
    public float shakeStrength = 0.5f;
    public int shakeVibrato = 10;

    [Header("Death Camera Settings")]
    public float deathCameraDelayDuration = 2.0f;   // Delay before camera starts moving
    public float deathCameraMoveDuration = 2.0f;     // Duration of camera movement on death
    public Ease deathCameraEase = Ease.OutQuad;     // Easing for death camera movement
    [Range(0.5f, 1f)]
    public float deathCameraScreenRatio = 0.75f;    // Move to 3/4 (75%) of screen height

    [Header("State")]
    public bool isFollowingPlayer = false;
    private bool isPlayerDead = false;

    private Vector3 velocity = Vector3.zero;
    private Tween currentTween;

    void OnEnable()
    {
        // Subscribe to player death event
        Health.PlayerDead += OnPlayerDead;
    }

    void OnDisable()
    {
        // Unsubscribe from player death event
        Health.PlayerDead -= OnPlayerDead;
    }

    void Start()
    {
        // Set initial position
        if (!isFollowingPlayer)
        {
            transform.position = defaultPosition;
        }
    }

    void Update()
    {
        // Hotkey: F to toggle follow mode
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    ToggleFollowMode();
        //}
    }

    void LateUpdate()
    {
        // Don't follow if player is dead
        if (isPlayerDead) return;

        // Smooth follow player if in follow mode
        if (isFollowingPlayer && player != null)
        {
            Vector3 targetPosition = player.position + followOffset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, followSmoothTime);
        }
    }

    #region Follow Mode

    // Toggle between follow player and default position
    public void ToggleFollowMode()
    {
        if (isFollowingPlayer)
        {
            SwitchToDefaultPosition();
        }
        else
        {
            SwitchToFollowPlayer();
        }
    }

    // Switch to follow player mode
    public void SwitchToFollowPlayer()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference is not set!");
            return;
        }

        isFollowingPlayer = true;
        
        // Kill previous tween
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        // Smooth transition to player position
        Vector3 targetPosition = player.position + followOffset;
        currentTween = transform.DOMove(targetPosition, switchDuration)
            .SetEase(switchEase)
            .OnComplete(() => velocity = Vector3.zero);
    }

    // Switch to default position
    public void SwitchToDefaultPosition()
    {
        isFollowingPlayer = false;

        // Kill previous tween
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        // Smooth transition to default position
        currentTween = transform.DOMove(defaultPosition, switchDuration)
            .SetEase(switchEase)
            .OnComplete(() => velocity = Vector3.zero);
    }

    #endregion

    #region Death Camera

    void OnPlayerDead()
    {
        if (isPlayerDead) return; // Prevent multiple calls
        isPlayerDead = true;

        Debug.Log("[CamController] Player died! Waiting before moving camera up...");

        // Stop following player
        isFollowingPlayer = false;

        // Kill any existing tween
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        // Wait for delay, then move camera
        DOVirtual.DelayedCall(deathCameraDelayDuration, () => {
            Debug.Log("[CamController] Delay complete, moving camera up...");

            // Calculate target position
            Vector3 targetPosition = CalculateDeathCameraPosition();

            // Smoothly move camera to death position
            currentTween = transform.DOMove(targetPosition, deathCameraMoveDuration)
                .SetEase(deathCameraEase)
                .OnComplete(() => {
                    velocity = Vector3.zero;
                    Debug.Log("[CamController] Death camera movement completed.");
                });
        });
    }

    Vector3 CalculateDeathCameraPosition()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("[CamController] Main Camera not found!");
            return transform.position + new Vector3(0, 5f, 0); // Fallback: move up 5 units
        }

        Vector3 currentPos = transform.position;

        // Calculate how much to move up based on screen ratio
        // deathCameraScreenRatio = 0.75 means move to show upper 3/4 of screen
        float screenHeightInWorld;

        if (cam.orthographic)
        {
            // Orthographic camera: direct calculation
            screenHeightInWorld = cam.orthographicSize * 2f;
        }
        else
        {
            // Perspective camera: calculate based on distance from camera
            float distance = Mathf.Abs(currentPos.z - cam.transform.position.z);
            if (distance < 0.1f) distance = 10f; // Fallback

            // Calculate screen height at the current Z plane
            float frustumHeight = 2.0f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            screenHeightInWorld = frustumHeight;
        }

        // Calculate upward offset
        // To move the camera so the current center becomes at 3/4 height position:
        // We need to move up by (deathCameraScreenRatio - 0.5) * screenHeight
        // For 0.75, this is (0.75 - 0.5) * screenHeight = 0.25 * screenHeight
        float upwardOffset = (deathCameraScreenRatio - 0.5f) * screenHeightInWorld;

        Vector3 targetPosition = currentPos + new Vector3(0, upwardOffset, 0);

        Debug.Log($"[CamController] Death camera calculation: Screen Height = {screenHeightInWorld:F2}, Upward Offset = {upwardOffset:F2}");
        Debug.Log($"[CamController] Moving from {currentPos} to {targetPosition}");

        return targetPosition;
    }

    #endregion

    #region Camera Shake

    // Trigger camera shake
    public void ShakeCamera()
    {
        ShakeCamera(shakeDuration, shakeStrength, shakeVibrato);
    }

    // Trigger camera shake with custom parameters
    public void ShakeCamera(float duration, float strength, int vibrato = 10)
    {
        transform.DOShakePosition(duration, strength, vibrato)
            .SetUpdate(true); // Works even when Time.timeScale = 0
    }

    // Trigger camera shake with randomness
    public void ShakeCameraRandom(float duration, float strength, int vibrato = 10, float randomness = 90f)
    {
        transform.DOShakePosition(duration, strength, vibrato, randomness)
            .SetUpdate(true);
    }

    #endregion

    #region Utility Methods

    // Set player reference at runtime
    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }

    // Stop all camera tweens
    public void StopAllTweens()
    {
        transform.DOKill();
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }
    }

    #endregion
}
