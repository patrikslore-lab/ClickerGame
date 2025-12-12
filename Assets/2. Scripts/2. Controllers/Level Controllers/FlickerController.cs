using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

/// <summary>
/// Handles flickering effect for Light2D during gameplay
/// </summary>
public class FlickerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light2D lightSettings;

    [Header("Flicker Settings")]
    [SerializeField] private float flickerFrequency = 1.0f;      // Speed of animation (how fast it changes)
    [SerializeField] private float flickerAmplitude = 0.2f;       // How much the light varies
    [SerializeField] private float flickerPhaseOffset = 0.0f;     // Starting position in noise field
    [SerializeField] private float noiseDetail = 1.0f;            // Detail level (1=smooth, higher=more variation)
    [SerializeField] private float noiseVariation = 0.0f;         // Different noise pattern (try 0, 50, 100, etc.)

    private PlayerConfig playerConfig;
    private bool isFlickering = false;

    private void Start()
    {
        if (lightSettings == null)
        {
            lightSettings = GetComponent<Light2D>();
        }

        if (lightSettings == null)
        {
            Debug.LogError("FlickerController requires a Light2D component!");
            enabled = false;
            return;
        }

        playerConfig = GameManager.Instance.GetPlayerConfig();
    }

    /// <summary>
    /// Start the flicker effect
    /// </summary>
    public void StartFlicker()
    {
        if (!isFlickering)
        {
            isFlickering = true;
            StartCoroutine(LightFlicker());
        }
    }

    /// <summary>
    /// Stop the flicker effect
    /// </summary>
    public void StopFlicker()
    {
        isFlickering = false;
        StopAllCoroutines();
    }

    private IEnumerator LightFlicker()
    {
        float time = flickerPhaseOffset; // Use phase offset as starting position

        while (isFlickering)
        {
            // Only apply flicker during gameplay
            if (GameManager.Instance != null && GameManager.Instance.IsInLevelGameplay)
            {
                // Sample Perlin noise with separated speed and detail
                // X axis: time-based animation (controlled by flickerFrequency)
                // Y axis: variation pattern (controlled by noiseVariation)
                float noiseX = time * flickerFrequency;
                float noiseY = noiseVariation + (time * noiseDetail);
                float noiseValue = Mathf.PerlinNoise(noiseX, noiseY);

                // Remap from [0,1] to [-1,1] for symmetric oscillation around the base radius
                float normalizedNoise = (noiseValue - 0.5f) * 2f;

                // Apply amplitude to get final offset
                float radiusOffset = flickerAmplitude * normalizedNoise;

                // Apply flicker offset to the current light health
                lightSettings.pointLightOuterRadius = playerConfig.lightHealthCurrent + radiusOffset;

                // Increment time
                time += Time.deltaTime;
            }

            yield return null; // Wait one frame
        }
    }
}
