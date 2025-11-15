using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
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
    public KeyCode jumpKey = KeyCode.Space;
    public bool isJumping = false;

    private Tween jumpTween;
    [Header("Shoot Settings")]
    public List<GameObject> bulletPrefabs;
    public List<int> bulletCounts;
    private Vector3 shootPoint;
    public float bulletSpeed = 10f;
    public int gunindex = 0;
    public bool isDebugging = true;

    [Header("Knockback Settings")]
    public float knockbackDecay = 5f; // How fast knockback decays

    // Event triggered when player shoots, passes gunindex
    public static Action<int> OnShoot;

    void Start()
    {
        BulletUI.Instance.BulletCount = bulletCounts[gunindex];
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
        shootPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        shootPoint.z = 0f; // important for 2D
        if (Input.GetMouseButtonDown(0))
        {
            if (gunindex > bulletCounts.Count || gunindex > bulletPrefabs.Count)
                return;
            if (bulletCounts[gunindex] > 0)
            {
                bulletCounts[gunindex]--;
                BulletUI.Instance.BulletCount = bulletCounts[gunindex];
                GameObject bulletPrefab = bulletPrefabs[gunindex];
                if (bulletPrefab != null)
                {
                    Vector3 shootpos;
                    if (visualTransform != null)
                        shootpos = visualTransform.position;
                    else
                        shootpos = transform.position;

                    GameObject bullet = Instantiate(bulletPrefab, shootpos, Quaternion.identity);
                    Vector2 shootDirection = (shootPoint - shootpos).normalized;
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
            visualTransform.DOLocalMoveY(0, jumpDuration * 0.5f)
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

        // Get screen corners in world space
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));
        Vector3 screenCenter = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, cam.nearClipPlane));

        // Set boundaries to lower half of screen
        minBounds.x = bottomLeft.x;
        maxBounds.x = bottomRight.x;
        minBounds.y = bottomLeft.y;
        maxBounds.y = screenCenter.y; // Use screen center as top boundary (lower half)

        Debug.Log($"Screen bounds calculated: Min({minBounds.x}, {minBounds.y}), Max({maxBounds.x}, {maxBounds.y})");
    }
}