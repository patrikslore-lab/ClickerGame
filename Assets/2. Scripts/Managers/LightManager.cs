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

        EventManager.Instance.LightDestruction += LightDestruction;
        EventManager.Instance.CoreHit += LightAddition;
        EventManager.Instance.ProtectorLightAddition += LightAdditionProtector;

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
        if (GameManager.Instance == null || !GameManager.Instance.IsInLevelGameplay)
            return;

        float reductionRate = GetReductionRateForEnemy(enemy);
        
        playerConfig.lightHealthCurrent -= reductionRate * Time.deltaTime;
        playerConfig.lightHealthCurrent = Mathf.Max(0f, playerConfig.lightHealthCurrent);
        
        // Check for game over when health actually changes (not every frame)
        if (playerConfig.lightHealthCurrent <= 0f && !isGameOver)
        {
            isGameOver = true;
            EventManager.Instance?.TriggerLightDepleted();
        }
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

}
