// LanternController.cs
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

/// <summary>
/// Unified controller for the Lantern - the player's light source.
/// Handles intro animation, flicker effect, and health (damage/healing).
///
/// Accessed by:
/// - LevelIntroController (intro phase: MoveToCenter, ActivateLight, SnapToFinalState)
/// - LevelManager (gameplay phase: StartGameplay, StopGameplay, ResetState, HandleCoreHit)
///
/// Responds to events:
/// - LightDestruction (enemy attacking - broadcast pattern)
/// - ProtectorLightAddition (protector ability active - broadcast pattern)
///
/// Direct calls from LevelManager:
/// - HandleCoreHit (player clicked enemy core - orchestrated call)
///
/// Triggers events:
/// - LightDepleted (health reached zero - game over)
/// </summary>
public class LanternController : MonoBehaviour
{
    //===========================================
    // SERIALIZED FIELDS
    //===========================================

    [Header("Lantern Prefab")]
    [SerializeField] private GameObject lanternPrefab;
    [SerializeField] private Vector3 spawnPosition = new Vector3(0f, -5f, 0f);

    [Header("Movement Settings (Intro)")]
    [SerializeField] private Vector3 targetPosition = Vector3.zero;
    [SerializeField] private float moveDuration = 1.5f;
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Light Settings (Intro)")]
    [SerializeField] private float lightFadeDuration = 1f;

    [Header("Flicker Settings (Gameplay)")]
    [SerializeField] private float flickerFrequency = 1.0f;
    [SerializeField] private float flickerAmplitude = 0.2f;
    [SerializeField] private float flickerPhaseOffset = 0.0f;
    [SerializeField] private float noiseDetail = 1.0f;
    [SerializeField] private float noiseVariation = 0.0f;

    //===========================================
    // PRIVATE FIELDS
    //===========================================

    private GameObject lanternInstance;
    private Light2D lightSettings;
    private PlayerConfig playerConfig;

    // Intro state
    private float savedLightValue;

    // Flicker state
    private bool isFlickering = false;
    private Coroutine flickerCoroutine;

    // Gameplay state
    private bool isGameplayActive = false;
    private bool isGameOver = false;

    //===========================================
    // PUBLIC ACCESSORS
    //===========================================

    public GameObject LanternInstance => lanternInstance;
    public bool IsSpawned => lanternInstance != null;

    //===========================================
    // UNITY LIFECYCLE
    //===========================================

    private void Start()
    {
        playerConfig = GameManager.Instance.GetPlayerConfig();
    }

    private void OnDisable()
    {
        if (isGameplayActive)
        {
            StopGameplay();
        }
    }

    //===========================================
    // SPAWNING
    //===========================================

    /// <summary>
    /// Instantiate the lantern at spawn position.
    /// Call this when entering a level.
    /// </summary>
    public void SpawnLantern()
    {
        if (lanternInstance != null)
        {
            Debug.LogWarning("LanternController: Lantern already spawned!");
            return;
        }

        if (lanternPrefab == null)
        {
            Debug.LogError("LanternController: Lantern prefab not assigned!");
            return;
        }

        lanternInstance = Instantiate(lanternPrefab, spawnPosition, Quaternion.identity);
        lightSettings = lanternInstance.GetComponent<Light2D>();

        if (lightSettings == null)
        {
            Debug.LogError("LanternController: Lantern prefab missing Light2D component!");
        }

        Debug.Log($"LanternController: Lantern spawned at {spawnPosition}");
    }

    /// <summary>
    /// Instantiate the lantern at a specific position.
    /// </summary>
    public void SpawnLantern(Vector3 position)
    {
        spawnPosition = position;
        SpawnLantern();
    }

    /// <summary>
    /// Destroy the lantern instance.
    /// Call this when leaving a level or returning to base.
    /// </summary>
    public void DespawnLantern()
    {
        if (lanternInstance == null) return;

        StopFlicker();
        Destroy(lanternInstance);
        lanternInstance = null;
        lightSettings = null;

        Debug.Log("LanternController: Lantern despawned");
    }

    //===========================================
    // INTRO PHASE (called by LevelIntroController)
    //===========================================

    /// <summary>
    /// Phase 1: Move lantern to center position.
    /// Called by LevelIntroController during intro sequence.
    /// </summary>
    public IEnumerator MoveToCenter()
    {
        if (lanternInstance == null)
        {
            Debug.LogError("LanternController: Cannot move - lantern not spawned!");
            yield break;
        }

        savedLightValue = playerConfig.lightHealthCurrent;
        if (lightSettings != null)
        {
            lightSettings.pointLightOuterRadius = 0;
        }

        float elapsed = 0f;
        Vector3 start = lanternInstance.transform.position;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = movementCurve.Evaluate(elapsed / moveDuration);
            lanternInstance.transform.position = Vector3.Lerp(start, targetPosition, t);
            yield return null;
        }

        lanternInstance.transform.position = targetPosition;
    }

    /// <summary>
    /// Phase 2: Fade in the light.
    /// Called by LevelIntroController during intro sequence.
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
        StartFlicker();
    }

    /// <summary>
    /// Called when intro is skipped - snap to final state immediately.
    /// Called by LevelIntroController.
    /// </summary>
    public void SnapToFinalState()
    {
        if (lanternInstance == null)
        {
            Debug.LogError("LanternController: Cannot snap - lantern not spawned!");
            return;
        }

        lanternInstance.transform.position = targetPosition;

        if (lightSettings != null)
        {
            lightSettings.enabled = true;
            lightSettings.pointLightOuterRadius = playerConfig.lightHealthCurrent;
        }

        StartFlicker();
    }

    //===========================================
    // GAMEPLAY PHASE (called by LevelManager)
    //===========================================

    /// <summary>
    /// Start gameplay systems - subscribe to damage/heal events.
    /// Called by LevelManager.StartCombatSession().
    /// </summary>
    public void StartGameplay()
    {
        if (isGameplayActive) return;

        isGameplayActive = true;
        isGameOver = false;

        SubscribeToEvents();
        StartFlicker();

        Debug.Log("LanternController: Gameplay started");
    }

    /// <summary>
    /// Stop gameplay systems - unsubscribe from events.
    /// Called by LevelManager.StopCombatSession().
    /// </summary>
    public void StopGameplay()
    {
        if (!isGameplayActive) return;

        isGameplayActive = false;

        UnsubscribeFromEvents();
        StopFlicker();

        Debug.Log("LanternController: Gameplay stopped");
    }

    /// <summary>
    /// Reset lantern state when returning to base or starting new level.
    /// Called by LevelManager.LoadLevel().
    /// </summary>
    public void ResetState()
    {
        isGameOver = false;

        if (lightSettings != null && playerConfig != null)
        {
            lightSettings.pointLightOuterRadius = playerConfig.lightHealthCurrent;
        }
    }

    //===========================================
    // EVENT SUBSCRIPTIONS (broadcast patterns only)
    //===========================================

    private void SubscribeToEvents()
    {
        if (EventManager.Instance == null) return;

        EventManager.Instance.LightDestruction += HandleLightDamage;
        EventManager.Instance.ProtectorLightAddition += HandleProtectorHeal;

        Debug.Log("LanternController: Subscribed to events");
    }

    private void UnsubscribeFromEvents()
    {
        if (EventManager.Instance == null) return;

        EventManager.Instance.LightDestruction -= HandleLightDamage;
        EventManager.Instance.ProtectorLightAddition -= HandleProtectorHeal;

        Debug.Log("LanternController: Unsubscribed from events");
    }

    //===========================================
    // DAMAGE / HEALING HANDLERS
    //===========================================

    private void HandleLightDamage(Enemy enemy)
    {
        if (!isGameplayActive || isGameOver) return;

        float reductionRate = GetReductionRateForEnemy(enemy);

        playerConfig.lightHealthCurrent -= reductionRate * Time.deltaTime;
        playerConfig.lightHealthCurrent = Mathf.Max(0f, playerConfig.lightHealthCurrent);

        if (playerConfig.lightHealthCurrent <= 0f && !isGameOver)
        {
            isGameOver = true;
            StopFlicker();
            EventManager.Instance?.TriggerLightDepleted();
            Debug.Log("LanternController: Light depleted - game over triggered");
        }
    }

    /// <summary>
    /// Handle core hit - add light reward for killing enemy.
    /// Called directly by LevelManager.HandleCoreHit().
    /// </summary>
    public void HandleCoreHit(Enemy enemy)
    {
        if (!isGameplayActive) return;

        float lightReward = GetRewardRateForEnemy(enemy);

        if (playerConfig.lightHealthCurrent < playerConfig.lightHealthMax)
        {
            playerConfig.lightHealthCurrent += lightReward;
            playerConfig.lightHealthCurrent = Mathf.Min(playerConfig.lightHealthMax, playerConfig.lightHealthCurrent);
            Debug.Log($"LanternController: Light reward +{lightReward} from {enemy.GetType().Name}");
        }
    }

    private void HandleProtectorHeal()
    {
        if (!isGameplayActive) return;

        if (playerConfig.lightHealthCurrent < playerConfig.lightHealthMax)
        {
            playerConfig.lightHealthCurrent += playerConfig.protectorLightAdditionRate * Time.deltaTime;
            playerConfig.lightHealthCurrent = Mathf.Min(playerConfig.lightHealthMax, playerConfig.lightHealthCurrent);
        }
    }

    //===========================================
    // ENEMY-SPECIFIC RATES
    //===========================================

    private float GetReductionRateForEnemy(Enemy enemy)
    {
        string enemyType = enemy.GetType().Name;

        return enemyType switch
        {
            "FlyingRat" => playerConfig.flyingRatLightReductionRate,
            "BringerOfDeathEnemy" => playerConfig.bringerOfDeathLightReductionRate,
            "Crowooon" => playerConfig.crowooonLightReductionRate,
            _ => 0.5f
        };
    }

    private float GetRewardRateForEnemy(Enemy enemy)
    {
        string enemyType = enemy.GetType().Name;

        return enemyType switch
        {
            "FlyingRat" => playerConfig.flyingRatLightRewardRate,
            "BringerOfDeathEnemy" => playerConfig.bringerOfDeathLightRewardRate,
            "Crowooon" => playerConfig.crowooonLightRewardRate,
            _ => 0.5f
        };
    }

    //===========================================
    // FLICKER EFFECT
    //===========================================

    private void StartFlicker()
    {
        if (isFlickering || lanternInstance == null) return;

        isFlickering = true;
        flickerCoroutine = StartCoroutine(FlickerLoop());
    }

    private void StopFlicker()
    {
        isFlickering = false;

        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
            flickerCoroutine = null;
        }

        if (lightSettings != null && playerConfig != null)
        {
            lightSettings.pointLightOuterRadius = playerConfig.lightHealthCurrent;
        }
    }

    private IEnumerator FlickerLoop()
    {
        float time = flickerPhaseOffset;

        while (isFlickering)
        {
            if (lightSettings != null && playerConfig != null)
            {
                float noiseX = time * flickerFrequency;
                float noiseY = noiseVariation + (time * noiseDetail);
                float noiseValue = Mathf.PerlinNoise(noiseX, noiseY);

                float normalizedNoise = (noiseValue - 0.5f) * 2f;
                float radiusOffset = flickerAmplitude * normalizedNoise;

                lightSettings.pointLightOuterRadius = playerConfig.lightHealthCurrent + radiusOffset;

                time += Time.deltaTime;
            }

            yield return null;
        }
    }
}