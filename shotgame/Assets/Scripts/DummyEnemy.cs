using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DummyEnemy : MonoBehaviour
{
    public GameObject head;

    [Header("Death Animation Settings")]
    public float bounceHeight1 = 2f;      // First bounce height
    public float bounceHeight2 = 1f;      // Second bounce height
    public float bounceHeight3 = 0.5f;    // Third bounce height
    public float bounceDuration = 0.4f;   // Duration per bounce
    public float horizontalDistance = 3f; // Total horizontal movement
    public float rotationSpeed = 720f;    // Degrees per second
    public float fadeOutDuration = 0.5f;  // Fade out time after landing

    private bool isDead = false;

    void Start()
    {

    }

    void Update()
    {
        // Test with T key
        if (Input.GetKeyDown(KeyCode.T) && !isDead)
        {
            Die();
        }
    }

    // Call this method when enemy dies
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (head != null)
        {
            PlayHeadBounceAnimation();
        }
        else
        {
            Debug.LogWarning("Head GameObject is not assigned!");
        }
    }

    void PlayHeadBounceAnimation()
    {
        // Detach head from parent
        head.transform.SetParent(null);

        Vector3 startPos = head.transform.position;
        Vector3 landingPos = startPos + Vector3.right * horizontalDistance;

        // Create animation sequence
        Sequence bounceSequence = DOTween.Sequence();

        // First bounce
        bounceSequence.Append(CreateBounceTween(head.transform, startPos,
            startPos + Vector3.right * (horizontalDistance * 0.4f), bounceHeight1, bounceDuration));

        // Second bounce
        bounceSequence.Append(CreateBounceTween(head.transform,
            startPos + Vector3.right * (horizontalDistance * 0.4f),
            startPos + Vector3.right * (horizontalDistance * 0.7f), bounceHeight2, bounceDuration * 0.8f));

        // Third bounce (small)
        bounceSequence.Append(CreateBounceTween(head.transform,
            startPos + Vector3.right * (horizontalDistance * 0.7f),
            landingPos, bounceHeight3, bounceDuration * 0.6f));

        // Roll a bit after landing
        bounceSequence.Append(head.transform.DOMoveX(landingPos.x + 0.5f, 0.3f).SetEase(Ease.OutQuad));

        // Rotation during entire animation
        float totalDuration = bounceDuration + (bounceDuration * 0.8f) + (bounceDuration * 0.6f) + 0.3f;
        head.transform.DORotate(new Vector3(0, 0, -rotationSpeed * totalDuration / 360f * 360f),
            totalDuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);

        // Fade out and destroy
        bounceSequence.OnComplete(() => {
            FadeOutAndDestroy();
        });
    }

    // Create a single bounce tween using parabola
    Tween CreateBounceTween(Transform target, Vector3 startPos, Vector3 endPos, float height, float duration)
    {
        Sequence bounce = DOTween.Sequence();

        // Horizontal movement (linear)
        bounce.Append(target.DOMove(endPos, duration).SetEase(Ease.Linear));

        // Vertical movement (parabola using OutQuad + InQuad)
        bounce.Join(target.DOMoveY(startPos.y + height, duration * 0.5f).SetEase(Ease.OutQuad));
        bounce.Append(target.DOMoveY(endPos.y, duration * 0.5f).SetEase(Ease.InQuad));

        return bounce;
    }

    // Fade out and destroy the head
    void FadeOutAndDestroy()
    {
        SpriteRenderer spriteRenderer = head.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.DOFade(0f, fadeOutDuration).OnComplete(() => {
                Destroy(head);
                Destroy(gameObject); // Destroy enemy body as well
            });
        }
        else
        {
            // If no sprite renderer, just destroy after delay
            Destroy(head, fadeOutDuration);
            Destroy(gameObject, fadeOutDuration);
        }
    }

    // Alternative method: Use coroutine instead of DoTween
    public void DieWithCoroutine()
    {
        if (isDead) return;
        isDead = true;

        if (head != null)
        {
            StartCoroutine(HeadBounceCoroutine());
        }
    }

    IEnumerator HeadBounceCoroutine()
    {
        head.transform.SetParent(null);

        Vector3 startPos = head.transform.position;
        float[] bounceHeights = { bounceHeight1, bounceHeight2, bounceHeight3 };
        float[] bounceDurations = { bounceDuration, bounceDuration * 0.8f, bounceDuration * 0.6f };
        float[] horizontalProgress = { 0.4f, 0.7f, 1f };

        // Perform bounces
        for (int i = 0; i < bounceHeights.Length; i++)
        {
            Vector3 bounceStart = head.transform.position;
            Vector3 bounceEnd = startPos + Vector3.right * (horizontalDistance * horizontalProgress[i]);

            yield return StartCoroutine(BounceTo(bounceStart, bounceEnd, bounceHeights[i], bounceDurations[i]));
        }

        // Final roll
        Vector3 finalPos = head.transform.position + Vector3.right * 0.5f;
        float rollTime = 0.3f;
        float elapsed = 0f;

        while (elapsed < rollTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rollTime;
            head.transform.position = Vector3.Lerp(head.transform.position, finalPos, t);
            head.transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
            yield return null;
        }

        // Fade out
        SpriteRenderer spriteRenderer = head.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            float fadeElapsed = 0f;
            Color startColor = spriteRenderer.color;

            while (fadeElapsed < fadeOutDuration)
            {
                fadeElapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, fadeElapsed / fadeOutDuration);
                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }
        }

        Destroy(head);
        Destroy(gameObject);
    }

    IEnumerator BounceTo(Vector3 start, Vector3 end, float height, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Horizontal linear movement
            float x = Mathf.Lerp(start.x, end.x, t);

            // Vertical parabolic movement
            float y = start.y + height * (4 * t * (1 - t)); // Parabola formula

            head.transform.position = new Vector3(x, y, head.transform.position.z);

            // Rotation
            head.transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);

            yield return null;
        }

        head.transform.position = end;
    }
}