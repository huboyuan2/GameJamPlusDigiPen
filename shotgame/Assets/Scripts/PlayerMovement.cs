using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Health health;
    private Vector2 moveInput;
    private Vector2 knockbackVelocity; // Store knockback velocity

    [Header("Movement Boundaries")]
    public bool useBoundaries = true;
    public bool useScreenBounds = false; // Auto calculate from screen
    public Vector2 minBounds = new Vector2(-10f, -10f);
    public Vector2 maxBounds = new Vector2(10f, 10f);

    [Header("Fake Jump Settings")]
    public Transform visualTransform; // Assign sprite child object here
    public float jumpHeight = 1.5f;
    
    public float jumpDuration = 0.5f;
    public float fallDuration = 0.5f;
    public KeyCode jumpKey = KeyCode.Space;
    public bool isJumping = false;
    private float originY;

    private Tween jumpTween;
    [Header("Shoot Settings")]
    public List<GameObject> bulletPrefabs;
    public List<int> bulletCounts;
    private Vector3 shootPoint;
    public float bulletSpeed = 10f;
    public int gunindex = 0;
    public bool isDebugging = true;
    public GameObject Arm;
    [Header("Knockback Settings")]
    public float knockbackDecay = 5f; // How fast knockback decays

    // Event triggered when player shoots, passes gunindex
    public static Action<int> OnShoot;

    void Start()
    {
        BulletUI.Instance.BulletCount = bulletCounts[gunindex];
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        // Key settings: disable gravity, freeze Z rotation
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Auto calculate boundaries from screen if enabled
        if (useScreenBounds)
        {
            CalculateScreenBounds();
        }

        // Auto find visual transform if not assigned
        if (visualTransform == null)
        {
            // Look for a child named "Visual" or use first child
            Transform visual = transform.Find("Visual");
            if (visual != null)
            {
                visualTransform = visual;
            }
            else if (transform.childCount > 0)
            {
                visualTransform = transform.GetChild(0);
                Debug.Log($"Auto-assigned visual transform: {visualTransform.name}");
            }
            else
            {
                Debug.LogWarning("No visual transform found! Please assign one or create a child object.");
            }
        }
        originY = visualTransform.position.y;
    }

    void Update()
    {
        // WASD or arrow key input
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Normalize to prevent faster diagonal movement
        moveInput = moveInput.normalized;

        // Jump input
        if (Input.GetKeyDown(jumpKey) && !isJumping)
        {
            PerformJump();
        }

        // Weapon switching
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f && bulletPrefabs.Count > 0 && bulletCounts.Count > 0)
        {
            int maxIndex = Mathf.Min(bulletPrefabs.Count, bulletCounts.Count) - 1;

            if (scrollInput > 0f)
            {
                // Scroll up - next weapon
                gunindex++;
                if (gunindex > maxIndex)
                {
                    gunindex = 0; // Wrap to first weapon
                }
            }
            else if (scrollInput < 0f)
            {
                // Scroll down - previous weapon
                gunindex--;
                if (gunindex < 0)
                {
                    gunindex = maxIndex; // Wrap to last weapon
                }
            }

            // Update UI when weapon changes
            BulletUI.Instance.BulletCount = bulletCounts[gunindex];
            Debug.Log($"Switched to weapon {gunindex}");
        }

        // Calculate shoot point and direction every frame
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(transform.position).z; // Keep original depth
        shootPoint = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        // Get shoot position
        Vector3 shootPos = visualTransform != null ? visualTransform.position : transform.position;
        
        // Calculate shoot direction
        Vector2 shootDirection = (shootPoint - shootPos).normalized;

        // Rotate Arm to face mouse direction every frame
        if (Arm != null)
        {
            float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
            Arm.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // Handle shooting
        if (Input.GetMouseButtonDown(0))
        {
            if (gunindex > bulletCounts.Count || gunindex > bulletPrefabs.Count)
                return;
            if (bulletCounts[gunindex] > 0)
            {
                if (isJumping)
                {
                    PerformJump();
                }
                bulletCounts[gunindex]--;
                BulletUI.Instance.BulletCount = bulletCounts[gunindex];
                GameObject bulletPrefab = bulletPrefabs[gunindex];
                if (bulletPrefab != null)
                {
                    GameObject bullet = Instantiate(bulletPrefab, shootPos, Quaternion.identity);
                    Bullet bulletScript = bullet.GetComponent<Bullet>();
                    if (bulletScript != null)
                    {
                        bulletScript.SetDirection(shootDirection);
                        if (bulletScript.oppositeForceStrength >= 0.1f)
                        {
                            // Apply knockback velocity (not AddForce, since we're overriding velocity in FixedUpdate)
                            knockbackVelocity += -shootDirection * bulletScript.oppositeForceStrength;
                            Debug.Log($"Knockback applied: {knockbackVelocity.magnitude}");
                        }
                    }

                    // Invoke shoot event
                    OnShoot?.Invoke(gunindex);
                }
            }
        }

        // Debug reload
        if (isDebugging)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                bulletCounts[gunindex] = 10;
                BulletUI.Instance.BulletCount = bulletCounts[gunindex];
            }
        }
    }

    void FixedUpdate()
    {
        // Combine movement input with knockback velocity
        Vector2 noYKnock = new Vector2(knockbackVelocity.x, 0);
        Vector2 targetVelocity = moveInput * moveSpeed + knockbackVelocity;
        rb.velocity = targetVelocity;

        // Decay knockback over time
        knockbackVelocity = Vector2.Lerp(knockbackVelocity, Vector2.zero, knockbackDecay * Time.fixedDeltaTime);

        // Clamp position within boundaries
        if (useBoundaries)
        {
            Vector2 clampedPosition = rb.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, minBounds.y, maxBounds.y);
            rb.position = clampedPosition;
        }
    }

    #region Fake Jump System

    // Perform fake jump animation
    void PerformJump()
    {
        if (visualTransform == null)
        {
            Debug.LogWarning("Cannot jump: visualTransform is not assigned!");
            return;
        }

        isJumping = true;

        // Kill previous jump tween if exists
        if (jumpTween != null && jumpTween.IsActive())
        {
            jumpTween.Kill();
        }

        // Create jump sequence
        Sequence jumpSequence = DOTween.Sequence();
        
        // Jump up
        jumpSequence.Append(
            visualTransform.DOLocalMoveY(jumpHeight, jumpDuration * 0.5f)
                .SetEase(Ease.OutQuad)
        );

        // Fall down
        jumpSequence.Append(
            visualTransform.DOLocalMoveY(0, fallDuration * 0.5f)
                .SetEase(Ease.InQuad)
        );

        // On complete
        jumpSequence.OnComplete(() => {
            isJumping = false;
        });

        jumpTween = jumpSequence;
    }

    // Public method to trigger jump from other scripts
    public void Jump()
    {
        if (!isJumping)
        {
            PerformJump();
        }
    }

    #endregion

    // Calculate boundaries from screen's lower half
    void CalculateScreenBounds()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("Main Camera not found! Using default bounds.");
            return;
        }

        // Determine the Z plane distance for boundary calculation
        float zDistance;

        if (cam.orthographic)
        {
            // Orthographic camera: use near clip plane
            zDistance = cam.nearClipPlane;
        }
        else
        {
            // Perspective camera: calculate Z distance from camera to player
            // Use player's current Z position, or a fixed gameplay plane Z
            zDistance = Mathf.Abs(cam.transform.position.z - transform.position.z);

            // Fallback: if player is too close to camera, use a reasonable default
            if (zDistance < 0.1f)
            {
                zDistance = 10f; // Default gameplay plane distance
                Debug.LogWarning($"Player too close to camera. Using default Z distance: {zDistance}");
            }
        }

        // Get screen corners in world space at the specified Z distance
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, zDistance));
        Vector3 bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0, zDistance));
        Vector3 screenCenter = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, zDistance));

        // Set boundaries to lower half of screen
        minBounds.x = -4f;//bottomLeft.x;
        maxBounds.x = 3.3f; //bottomRight.x;
        minBounds.y = -2.2f;// bottomLeft.y;
        maxBounds.y = 0.25f;// screenCenter.y; // Use screen center as top boundary (lower half)

        Debug.Log($"Screen bounds calculated (Camera: {(cam.orthographic ? "Orthographic" : "Perspective")}, Z Distance: {zDistance:F2}): Min({minBounds.x:F2}, {minBounds.y:F2}), Max({maxBounds.x:F2}, {maxBounds.y:F2})");
    }
}