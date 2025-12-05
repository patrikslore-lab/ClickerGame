using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Controls the lantern during level intro - Phase 1
/// </summary>
public class LanternController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Vector3 targetPosition = Vector3.zero;
    [SerializeField] private float moveDuration = 1.5f;
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool isSubscribed = false;
    private UnityEngine.Rendering.Universal.Light2D lightSettings;
    private FlickerController flickerController;
    private float savedLightValue;
    PlayerConfig playerConfig;

    Light2D lightSetting;

    private void Start()
    {
        playerConfig = GameManager.Instance.GetPlayerConfig();
        lightSetting = GetComponent<Light2D>();
        // Get components on same GameObject
        lightSettings = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        flickerController = GetComponent<FlickerController>();

        // Try subscribing in Start() in case EventManager wasn't ready during OnEnable()
        TrySubscribe();

        // Store and set light to 0 when entering a level (will be activated during Phase 2)
    }

    private void OnEnable()
    {
        Debug.Log("LanternController: OnEnable called");
        TrySubscribe();
    }

    private void TrySubscribe()
    {
        // Don't subscribe twice
        if (isSubscribed) return;

        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnIntroPhase1_LanternEnter += HandlePhase1;
            EventManager.Instance.OnIntroPhase2_LightActivate += HandlePhase2;
            isSubscribed = true;
            Debug.Log("LanternController: Successfully subscribed to intro events");
        }
        else
        {
            Debug.LogWarning("LanternController: EventManager not ready yet, will retry in Start()");
        }
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null && isSubscribed)
        {
            EventManager.Instance.OnIntroPhase1_LanternEnter -= HandlePhase1;
            EventManager.Instance.OnIntroPhase2_LightActivate -= HandlePhase2;
            isSubscribed = false;
        }
    }

    private void HandlePhase1()
    {
        Debug.Log("LanternController: Starting Phase 1");
        StartCoroutine(MoveToCenter());
    }

    private IEnumerator MoveToCenter()
    {
        savedLightValue = playerConfig.lightHealthCurrent;
        Debug.Log($"{savedLightValue} stored");
        lightSetting.pointLightOuterRadius = 0;
        float elapsed = 0f;
        Vector3 start = transform.position;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;
            float curveValue = movementCurve.Evaluate(t);

            transform.position = Vector3.Lerp(start, targetPosition, curveValue);

            yield return null;
        }

        transform.position = targetPosition;

        Debug.Log("LanternController: Movement complete, announcing Phase 1 complete");
        EventManager.Instance.TriggerIntroPhase1Complete();
    }

    private void HandlePhase2()
    {
        Debug.Log("LanternController: Starting Phase 2 - activating light");
        StartCoroutine(ActivateLightSequence());
    }

    private IEnumerator ActivateLightSequence()
    {
        // Fade in the light from 0 to the saved value
        if (lightSettings != null)
        {
            float elapsed = 0f;
            float duration = 1f;
            float targetIntensity = savedLightValue;

            lightSettings.enabled = true;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                lightSetting.pointLightOuterRadius = Mathf.Lerp(0, targetIntensity, t);
                yield return null;
            }

            lightSettings.pointLightOuterRadius = targetIntensity;
        }

        Debug.Log($"LanternController: Light activation complete, restored to {savedLightValue}");

        // Start flickering after light is activated
        if (flickerController != null)
        {
            flickerController.StartFlicker();
        }

        yield return new WaitForSeconds(1);
        EventManager.Instance.TriggerIntroPhase2Complete();
    }
}