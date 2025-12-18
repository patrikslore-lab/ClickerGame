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
    private SpriteRenderer gradeSpriteRenderer;

    /// <summary>
    /// Initialize popup with grade sprite and color
    /// </summary>
    /// <param name="gradeSprite">The sprite to display (S, A, B, C, or D)</param>
    /// <param name="gradeColor">Color tint for the sprite</param>
    /// <param name="worldPosition">Where to spawn in world space</param>
    public void Initialize(Vector3 worldPosition)
    {
        gradeSpriteRenderer = GetComponent<SpriteRenderer>();

        if (gradeSpriteRenderer == null)
        {
            Debug.LogError($"GradePopup: SpriteRenderer component not found on {gameObject.name}!");
            return;
        }

        if (gradeSpriteRenderer.sprite == null)
        {
            Debug.LogError($"GradePopup: SpriteRenderer on {gameObject.name} has no sprite assigned!");
            return;
        }

        Debug.Log($"GradePopup: Successfully initialized {gameObject.name} with sprite '{gradeSpriteRenderer.sprite.name}'");

        transform.position = worldPosition;
        StartCoroutine(FloatAndFade());
    }

    private IEnumerator FloatAndFade()
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < lifetime)
        {
            // Float upward
            transform.position = startPosition + Vector3.up * (floatSpeed * elapsedTime);

            // Fade out in last portion of lifetime
            if (elapsedTime >= fadeStartTime && gradeSpriteRenderer != null)
            {
                float fadeProgress = (elapsedTime - fadeStartTime) / (lifetime - fadeStartTime);
                Color currentColor = gradeSpriteRenderer.color;
                currentColor.a = 1f - fadeProgress;
                gradeSpriteRenderer.color = currentColor;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
