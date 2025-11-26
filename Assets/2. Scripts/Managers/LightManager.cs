using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.UI;

public class LightManager : MonoBehaviour
{
    private Light2D lightSettings;

    private float lightReductionRate;

    private float lightRewardRate;


    [SerializeField] Slider LightHealth;

    private float lightHealthNumberMax;
    
    [SerializeField] float lightHealthNumber;

    private PlayerConfig playerConfig;

    private float timeTaken;
 
    private bool isGameOver = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lightSettings = GetComponent<Light2D>();
        EventManager.Instance.LightDestruction += LightDestruction;
        EventManager.Instance.CoreHit += LightAddition;
        lightHealthNumberMax = (lightSettings.pointLightOuterRadius + lightSettings.pointLightInnerRadius)/2;
        playerConfig = GameManager.Instance.GetPlayerConfig();
    }

    // Update is called once per frame
    void Update()
    {
        // Only run light mechanics during Combat mode
        if (GameManager.Instance.CurrentGameMode != GameManager.GameMode.Combat)
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
    }

    private void GameOver()
    {
        Debug.Log("Player defeated - returning to base");

        // Reset game over flag for next combat
        isGameOver = false;

        // Set game state to GameOver (will show game over panel)
        GameManager.Instance.SetGameState(GameManager.GameState.GameOver);

        // Optional: Reset player to level 1 on death (roguelike style)
        // Uncomment if you want death to reset progress:
        // PlayerConfig playerConfig = GameManager.Instance.GetPlayerConfig();
        // playerConfig.currentLevel = 1;

        // Return to base area after a brief delay (so player can see game over screen)
        // Or you can add a "Return to Base" button on the game over panel
        // For now, we'll just set the state - add button later to call LoadBaseArea()
    }

    void DisplayLightHealth()
    {
        lightHealthNumber = (lightSettings.pointLightOuterRadius + lightSettings.pointLightInnerRadius)/2;
        LightHealth.value = lightHealthNumber / lightHealthNumberMax;
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
        if (GameManager.Instance.CurrentGameMode != GameManager.GameMode.Combat)
            return;

        // Determine reduction rate based on enemy type
        float reductionRate = GetReductionRateForEnemy(enemy);

        lightSettings.pointLightOuterRadius -= reductionRate * Time.deltaTime;
        lightSettings.pointLightInnerRadius -= reductionRate * Time.deltaTime;
    }

    void LightAddition(Enemy enemy)
    {
        // Only process light rewards during Combat mode
        if (GameManager.Instance.CurrentGameMode != GameManager.GameMode.Combat)
            return;

        float lightReward = GetRewardRateForEnemy(enemy);

        if (lightHealthNumber < lightHealthNumberMax)
        {
            lightSettings.pointLightOuterRadius += lightReward;
            lightSettings.pointLightInnerRadius += lightReward;
        }
    }
}
