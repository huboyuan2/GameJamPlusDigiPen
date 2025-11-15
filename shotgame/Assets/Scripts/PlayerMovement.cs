using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

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
    private Vector3 shootPoint;
    public float bulletSpeed = 10f;

    void Start()
    {
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
        shootPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        shootPoint.z = 0f; // important for 2D
        if (Input.GetMouseButtonDown(0))
        {
            GameObject bulletPrefab = bulletPrefabs[0];
            if (bulletPrefab != null)
            {
                GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                Vector2 shootDirection = (shootPoint - transform.position).normalized;
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.SetDirection(shootDirection);
                }
            }
        }
    }

    void FixedUpdate()
    {
        // Directly set velocity (DNF-style immediate response)
        rb.velocity = moveInput * moveSpeed;

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