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


    [SerializeField] Slider LightHealth;

    [Header("Light Flicker Settings")]
    [SerializeField] private float flickerFrequency = 1.0f;      // Speed of animation (how fast it changes)
    [SerializeField] private float flickerAmplitude = 0.2f;       // How much the light varies
    [SerializeField] private float flickerPhaseOffset = 0.0f;     // Starting position in noise field
    [SerializeField] private float noiseDetail = 1.0f;            // Detail level (1=smooth, higher=more variation)
    [SerializeField] private float noiseVariation = 0.0f;         // Different noise pattern (try 0, 50, 100, etc.)

    private PlayerConfig playerConfig;

    private bool isGameOver = false;

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

        EventManager.Instance.LightDestruction += LightDestruction;
        EventManager.Instance.CoreHit += LightAddition;
        EventManager.Instance.ProtectorLightAddition += LightAdditionProtector;

        playerConfig = GameManager.Instance.GetPlayerConfig();

        // Initialize from Light2D if PlayerConfig not yet set
        if (playerConfig.lightHealthMax <= 0)
        {
            playerConfig.lightHealthMax = lightSettings.pointLightOuterRadius;
        }
        if (playerConfig.lightHealthCurrent <= 0)
        {
            playerConfig.lightHealthCurrent = playerConfig.lightHealthMax;
        }

        StartCoroutine(LightFlicker(lightSettings));
    }

    // Update is called once per frame
    void Update()
    {
        // Only run light mechanics during Combat mode
        if (GameManager.Instance == null || GameManager.Instance.CurrentGameMode != GameManager.GameMode.Combat)
            return;

        DisplayLightHealth();
        if (LightHealth.value <= 0 && !isGameOver)
        {
            isGameOver = true;
            GameOver();
        }
    }

    private void OnDestroy()
    {
        EventManager.Instance.LightDestruction -= LightDestruction;
        EventManager.Instance.CoreHit -= LightAddition;
        EventManager.Instance.ProtectorLightAddition -= LightAdditionProtector;
    }

    private void GameOver()
    {
        Debug.Log("Player defeated - returning to base");

        // Reset game over flag for next combat
        isGameOver = false;

        // Set game state to GameOver (will show game over panel)
        GameManager.Instance.SetGameState(GameManager.GameState.GameOver);

        PlayerConfig playerConfig = GameManager.Instance.GetPlayerConfig();
        playerConfig.currentLevel = 1;
        playerConfig.lightHealthCurrent = playerConfig.lightHealthMax; // Reset to full health

        // Return to base area after a brief delay (so player can see game over screen)
        // Or you can add a "Return to Base" button on the game over panel
        // For now, we'll just set the state - add button later to call LoadBaseArea()
    }

    void DisplayLightHealth()
    {
        // Use actual health (without flicker) for health calculation
        LightHealth.value = playerConfig.lightHealthCurrent / playerConfig.lightHealthMax;
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
        // Only process light damage during Combat mode
        if (GameManager.Instance == null || GameManager.Instance.CurrentGameMode != GameManager.GameMode.Combat)
            return;

        // Determine reduction rate based on enemy type
        float reductionRate = GetReductionRateForEnemy(enemy);

        // Modify light health (flicker will be applied on top)
        playerConfig.lightHealthCurrent -= reductionRate * Time.deltaTime;
        playerConfig.lightHealthCurrent = Mathf.Max(0f, playerConfig.lightHealthCurrent);
    }

    void LightAddition(Enemy enemy)
    {
        // Only process light rewards during Combat mode
        if (GameManager.Instance == null || GameManager.Instance.CurrentGameMode != GameManager.GameMode.Combat)
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
        // Only process light rewards during Combat mode
        if (GameManager.Instance == null || GameManager.Instance.CurrentGameMode != GameManager.GameMode.Combat)
            return;

        if (playerConfig.lightHealthCurrent < playerConfig.lightHealthMax)
        {
            // Modify light health (flicker will be applied on top)
            playerConfig.lightHealthCurrent += playerConfig.protectorLightAdditionRate * Time.deltaTime;
            playerConfig.lightHealthCurrent = Mathf.Min(playerConfig.lightHealthMax, playerConfig.lightHealthCurrent);
        }
    }

    IEnumerator LightFlicker(Light2D lightSettings)
    {
        float time = flickerPhaseOffset; // Use phase offset as starting position

        while(true)
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

            yield return null; // Wait one frame
        }
    }
}
