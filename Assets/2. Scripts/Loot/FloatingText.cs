using System.Collections;
using UnityEngine;
using System;

public class FloatingText : MonoBehaviour
{
    private float floatSpeed = 0.5f;
    private float lifetime = 1.5f;
    private Action onLifetimeComplete;

    /// <summary>
    /// Initializes the floating text with animation parameters
    /// </summary>
    /// <param name="speed">How fast the text floats upward</param>
    /// <param name="life">How long the text should exist before disappearing</param>
    /// <param name="onComplete">Optional callback when lifetime expires</param>
    public void Initialize()
    {
        StartCoroutine(FloatAndDisappear());
    }

    IEnumerator FloatAndDisappear()
    {
        float elapsedTime = 0f;

        while (elapsedTime < lifetime)
        {
            transform.position += Vector3.up * floatSpeed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Invoke callback if provided
        onLifetimeComplete?.Invoke();

        // Only destroy if not being pooled (callback handles pooling)
        if (onLifetimeComplete == null)
        {
            Destroy(gameObject);
        }
    }
}