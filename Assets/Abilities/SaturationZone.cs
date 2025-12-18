using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SaturationZone : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Leave empty to use this GameObject's position")]
    public Transform zoneSource;
    
    [Tooltip("The Light2D whose outer radius defines the saturation zone")]
    public Light2D linkedLight;
    
    [Tooltip("The Renderer with the desaturation material")]
    public Renderer targetRenderer;
    
    [Header("Settings")]
    public bool isActive = true;
    
    [Tooltip("Multiplier for the light radius (1.0 = exact match)")]
    [Range(0.1f, 2f)]
    public float radiusMultiplier = 1f;
    
    [Header("Debug")]
    public bool showDebugInfo = false;
    
    static readonly int ZoneCenterID = Shader.PropertyToID("_SaturationZoneCenter");
    static readonly int ZoneRadiusID = Shader.PropertyToID("_SaturationZoneRadius");
    
    private MaterialPropertyBlock propBlock;
    private Material targetMaterial;
    
    void Start()
    {
        // If no zone source specified, use this GameObject
        if (zoneSource == null)
        {
            zoneSource = transform;
        }
        
        // Try to auto-find Light2D on this GameObject if not assigned
        if (linkedLight == null)
        {
            linkedLight = GetComponent<Light2D>();
        }
        
        // Initialize property block
        propBlock = new MaterialPropertyBlock();
        
        if (showDebugInfo)
        {
            Debug.Log($"SaturationZone initialized on {gameObject.name}");
            Debug.Log($"Target Renderer: {(targetRenderer != null ? targetRenderer.name : "NULL")}");
        }
    }
    
    void Update() // Changed to Update for immediate response
    {
        if (!isActive || zoneSource == null)
        {
            if (targetMaterial != null)
            {
                targetMaterial.SetFloat(ZoneRadiusID, 0);
            }
            Shader.SetGlobalFloat(ZoneRadiusID, 0);
            return;
        }
        
        // Get radius from Light2D outer radius
        float radius = 3f;
        if (linkedLight != null)
        {
            radius = linkedLight.pointLightOuterRadius * radiusMultiplier;
        }
        
        // Get world position
        Vector3 pos = zoneSource.position;
        Vector4 centerPos = new Vector4(pos.x, pos.y, 0, 0);
        
        // Set properties on the material directly
        if (targetMaterial != null)
        {
            targetMaterial.SetVector(ZoneCenterID, centerPos);
            targetMaterial.SetFloat(ZoneRadiusID, radius);
        }
        
        // Also set globally (in case other materials use it)
        Shader.SetGlobalVector(ZoneCenterID, centerPos);
        Shader.SetGlobalFloat(ZoneRadiusID, radius);
        
        if (showDebugInfo)
        {
            Debug.Log($"Frame {Time.frameCount} - Pos: ({pos.x:F2}, {pos.y:F2}), Radius: {radius:F2}");
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Transform source = zoneSource != null ? zoneSource : transform;
        
        Gizmos.color = Color.yellow;
        float radius = linkedLight != null 
            ? linkedLight.pointLightOuterRadius * radiusMultiplier 
            : 3f;
        
        // Draw the saturation zone
        Gizmos.DrawWireSphere(source.position, radius);
        
        // Draw a line showing direction if moving
        Gizmos.color = Color.green;
        Gizmos.DrawLine(source.position, source.position + Vector3.right * 0.5f);
    }
}