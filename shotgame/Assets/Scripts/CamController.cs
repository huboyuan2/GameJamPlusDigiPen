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

    [Header("Default Position")]
    public Vector3 defaultPosition = new Vector3(0, 0, -10f);
    public float switchDuration = 0.5f;
    public Ease switchEase = Ease.OutCubic;

    [Header("Camera Shake")]
    public float shakeDuration = 0.3f;
    public float shakeStrength = 0.5f;
    public int shakeVibrato = 10;

    [Header("State")]
    public bool isFollowingPlayer = false;

    private Vector3 velocity = Vector3.zero;
    private Tween currentTween;

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
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFollowMode();
        }
    }

    void LateUpdate()
    {
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
