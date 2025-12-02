using System.Collections;
using UnityEngine;

/// <summary>
/// Sprite-based grade popup (S/A/B/C/D) that appears at enemy position.
/// Floats upward and fades out. Uses SpriteRenderer for world-space rendering.
/// </summary>
public class GradePopup : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private float lifetime = 1.2f;
    [SerializeField] private float fadeStartTime = 0.8f;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Initialize popup with grade sprite and color
    /// </summary>
    /// <param name="gradeSprite">The sprite to display (S, A, B, C, or D)</param>
    /// <param name="gradeColor">Color tint for the sprite</param>
    /// <param name="worldPosition">Where to spawn in world space</param>
    public void Initialize(Sprite gradeSprite, Vector3 worldPosition)
    {
        transform.position = worldPosition;

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = gradeSprite;
        }

        StartCoroutine(FloatAndFade());
    }

    private IEnumerator FloatAndFade()
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Color startColor = spriteRenderer != null ? spriteRenderer.color : Color.white;

        while (elapsedTime < lifetime)
        {
            // Float upward
            transform.position = startPosition + Vector3.up * (floatSpeed * elapsedTime);

            // Fade out in last portion of lifetime
            if (elapsedTime >= fadeStartTime && spriteRenderer != null)
            {
                float fadeProgress = (elapsedTime - fadeStartTime) / (lifetime - fadeStartTime);
                Color currentColor = startColor;
                currentColor.a = 1f - fadeProgress;
                spriteRenderer.color = currentColor;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
