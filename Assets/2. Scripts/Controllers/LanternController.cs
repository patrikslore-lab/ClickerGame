// LanternController.cs
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class LanternController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Vector3 targetPosition = Vector3.zero;
    [SerializeField] private float moveDuration = 1.5f;
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Light Settings")]
    [SerializeField] private float lightFadeDuration = 1f;

    private Light2D lightSettings;
    private FlickerController flickerController;
    private PlayerConfig playerConfig;
    private float savedLightValue;

    private void Start()
    {
        playerConfig = GameManager.Instance.GetPlayerConfig();
        lightSettings = GetComponent<Light2D>();
        flickerController = GetComponent<FlickerController>();
    }

    /// <summary>
    /// Phase 1: Move lantern to center position
    /// Called by LevelIntroController
    /// </summary>
    public IEnumerator MoveToCenter()
    {
        // Store and zero out light for dramatic reveal later
        savedLightValue = playerConfig.lightHealthCurrent;
        if (lightSettings != null)
        {
            lightSettings.pointLightOuterRadius = 0;
        }

        float elapsed = 0f;
        Vector3 start = transform.position;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = movementCurve.Evaluate(elapsed / moveDuration);
            transform.position = Vector3.Lerp(start, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
    }

    /// <summary>
    /// Phase 2: Fade in the light
    /// Called by LevelIntroController
    /// </summary>
    public IEnumerator ActivateLight()
    {
        if (lightSettings == null) yield break;

        float elapsed = 0f;
        lightSettings.enabled = true;

        while (elapsed < lightFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lightFadeDuration;
            lightSettings.pointLightOuterRadius = Mathf.Lerp(0, savedLightValue, t);
            yield return null;
        }

        lightSettings.pointLightOuterRadius = savedLightValue;

        // Start flickering after light is fully on
        flickerController?.StartFlicker();
    }

    /// <summary>
    /// Called when intro is skipped - snap to final state immediately
    /// </summary>
    public void SnapToFinalState()
    {
        transform.position = targetPosition;
        
        if (lightSettings != null)
        {
            lightSettings.enabled = true;
            lightSettings.pointLightOuterRadius = playerConfig.lightHealthCurrent;
        }
        
        flickerController?.StartFlicker();
    }
}