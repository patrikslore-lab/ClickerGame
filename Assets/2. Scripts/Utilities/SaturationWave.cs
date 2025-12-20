using UnityEngine;

/// <summary>
/// Expanding saturation wave effect component.
/// Creates a donut-shaped ring that grows outward and saturates everything in its path.
/// Self-destructs when the wave expands beyond the maximum radius.
/// </summary>
public class SaturationWave : MonoBehaviour
{
    private float innerRadius = 0f;
    private float outerRadius;
    private float expansionSpeed;
    private float maxRadius;
    private Vector3 center;

    // Shader property IDs - must match EXACT names in shader graph (with underscore prefix)
    private static readonly int WaveCenterID = Shader.PropertyToID("_SaturationWaveCenter");
    private static readonly int InnerRadiusID = Shader.PropertyToID("_SaturationWaveInnerRadius");
    private static readonly int OuterRadiusID = Shader.PropertyToID("_SaturationWaveOuterRadius");

    /// <summary>
    /// Initialize the wave with position and settings.
    /// Called by SaturationWaveController after instantiation.
    /// </summary>
    public void Initialize(Vector2 position, float speed, float thickness, float max)
    {
        center = position;
        expansionSpeed = speed;
        maxRadius = max;

        // Initialize the donut ring with proper thickness
        innerRadius = 0f;
        outerRadius = thickness;
    }


    void Update()
    {
        // Expand the ring
        innerRadius += expansionSpeed * Time.deltaTime;
        outerRadius += expansionSpeed * Time.deltaTime;

        // Mirror to shader
        Shader.SetGlobalVector(WaveCenterID, center);
        Shader.SetGlobalFloat(InnerRadiusID, innerRadius);
        Shader.SetGlobalFloat(OuterRadiusID, outerRadius);

        Debug.LogWarning($"🔴 WAVE UPDATE - Inner: {innerRadius:F2}, Outer: {outerRadius:F2}, Center: ({center.x:F2}, {center.y:F2})");

        // Despawn when off-screen
        if (outerRadius > maxRadius)
        {
            Debug.Log($"✗ SaturationWave destroyed - reached max radius {maxRadius}");
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        // Clean up shader properties
        Shader.SetGlobalFloat(InnerRadiusID, 0);
        Shader.SetGlobalFloat(OuterRadiusID, 0);
    }
}
