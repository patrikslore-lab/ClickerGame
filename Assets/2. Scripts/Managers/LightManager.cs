using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;

public class LightManager : MonoBehaviour
{
    private Light2D lightSettings;

    private float lightReductionRate;

    private float lightRewardRate;

    [Header("References")]
    [SerializeField] private FlickerController flickerController;

    private PlayerConfig playerConfig;

    private bool isGameOver = false;
    private bool isSubscribed = false;

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null && isSubscribed)
        {
            EventManager.Instance.LightDestruction -= LightDestruction;
            EventManager.Instance.CoreHit -= LightAddition;
            EventManager.Instance.ProtectorLightAddition -= LightAdditionProtector;
            isSubscribed = false;
        }
    }

    private void TrySubscribe()
    {
        if (isSubscribed) return;

        if (EventManager.Instance != null)
        {
            EventManager.Instance.LightDestruction += LightDestruction;
            EventManager.Instance.CoreHit += LightAddition;
            EventManager.Instance.ProtectorLightAddition += LightAdditionProtector;
            isSubscribed = true;
            Debug.Log("LightManager: Successfully subscribed to events");
        }
        else
        {
            Debug.LogWarning("LightManager: EventManager not ready yet, will retry in Start()");
        }
    }

    void Start()
    {
        lightSettings = GetComponent<Light2D>();

        // Add null check for safety
        if (lightSettings == null)
        {
            Debug.LogError("LightManager requires a Light2D component!");
            enabled = false;
            return;
        }

        playerConfig = GameManager.Instance.GetPlayerConfig();

        // Try subscribing in Start() in case EventManager wasn't ready during OnEnable()
        TrySubscribe();

        // Get FlickerController if not assigned
        if (flickerController == null)
        {
            flickerController = GetComponent<FlickerController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Only run light mechanics during LevelGameplay state
        if (GameManager.Instance == null || !GameManager.Instance.IsInLevelGameplay)
            return;

        if (playerConfig.lightHealthCurrent <= 0 && !isGameOver)
        {
            isGameOver = true;
            GameOver();
        }
    }
    private void GameOver()
    {
        Debug.Log("Player defeated - returning to base");
        isGameOver = false;
        GameManager.Instance.TransitionToGameOver();
        playerConfig.currentLevel = 0;
    }

    private float GetReductionRateForEnemy(Enemy enemy)
    {
        // Check enemy type by class name
        string enemyType = enemy.GetType().Name;

        switch (enemyType)
        {
            case "FlyingRat":
                return playerConfig.flyingRatLightReductionRate;
            case "BringerOfDeathEnemy":
                return playerConfig.bringerOfDeathLightReductionRate;
            case "Crowooon":
                return playerConfig.crowooonLightReductionRate;
            default:
                return lightReductionRate;  // Default fallback
        }
    }

        private float GetRewardRateForEnemy(Enemy enemy)
    {
        // Check enemy type by class name
        string enemyType = enemy.GetType().Name;

        switch (enemyType)
        {
            case "FlyingRat":
                return playerConfig.flyingRatLightRewardRate;
            case "BringerOfDeathEnemy":
                return playerConfig.bringerOfDeathLightRewardRate;
            case "Crowooon":
                return playerConfig.crowooonLightRewardRate;
            default:
                return lightRewardRate;  // Default fallback
        }
    }
    public void LightDestruction(Enemy enemy)
    {
        // Only process light damage during LevelGameplay state
        if (GameManager.Instance == null || !GameManager.Instance.IsInLevelGameplay)
            return;

        // Determine reduction rate based on enemy type
        float reductionRate = GetReductionRateForEnemy(enemy);

        // Modify light health (flicker will be applied on top)
        playerConfig.lightHealthCurrent -= reductionRate * Time.deltaTime;
        playerConfig.lightHealthCurrent = Mathf.Max(0f, playerConfig.lightHealthCurrent);
    }

    void LightAddition(Enemy enemy)
    {
        // Only process light rewards during LevelGameplay state
        if (GameManager.Instance == null || !GameManager.Instance.IsInLevelGameplay)
            return;

        float lightReward = GetRewardRateForEnemy(enemy);

        if (playerConfig.lightHealthCurrent < playerConfig.lightHealthMax)
        {
            // Modify light health (flicker will be applied on top)
            playerConfig.lightHealthCurrent += lightReward;
            playerConfig.lightHealthCurrent = Mathf.Min(playerConfig.lightHealthMax, playerConfig.lightHealthCurrent);
        }
    }

    public void LightAdditionProtector()
    {
        // Only process light rewards during LevelGameplay state
        if (GameManager.Instance == null || !GameManager.Instance.IsInLevelGameplay)
            return;

        if (playerConfig.lightHealthCurrent < playerConfig.lightHealthMax)
        {
            // Modify light health (flicker will be applied on top)
            playerConfig.lightHealthCurrent += playerConfig.protectorLightAdditionRate * Time.deltaTime;
            playerConfig.lightHealthCurrent = Mathf.Min(playerConfig.lightHealthMax, playerConfig.lightHealthCurrent);
        }
    }

}
