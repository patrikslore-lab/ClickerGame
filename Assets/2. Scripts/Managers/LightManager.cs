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
        // Create Canvas
        GameObject canvasObj = new GameObject("GameOverCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.transform.position = Vector3.zero;
        
        // Create TextMeshPro text
        GameObject textObj = new GameObject("GameOverText");
        textObj.transform.SetParent(canvasObj.transform);
        textObj.transform.localPosition = new Vector3(0, 0, -2);
        //textObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        
        TextMeshProUGUI textMeshPro = textObj.AddComponent<TextMeshProUGUI>();
        textMeshPro.text = "GAME OVER";
        textMeshPro.alignment = TextAlignmentOptions.Center;
        textMeshPro.color = Color.red;
        textMeshPro.fontSize = 60;  // Large font size = sharp text
        textObj.layer = LayerMask.NameToLayer("UI");
        
        // Set size
        RectTransform rectTransform = textObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(400, 200);
        
        
        Debug.Log("Game Over!");
        Time.timeScale = 0f;
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
            default:
                return lightRewardRate;  // Default fallback
        }
    }
    public void LightDestruction(Enemy enemy)
    {
        // Determine reduction rate based on enemy type
        float reductionRate = GetReductionRateForEnemy(enemy);

        lightSettings.pointLightOuterRadius -= reductionRate * Time.deltaTime;
        lightSettings.pointLightInnerRadius -= reductionRate * Time.deltaTime;
    }

    void LightAddition(Enemy enemy)
    {
        float lightReward = GetRewardRateForEnemy(enemy);

        if (lightHealthNumber < lightHealthNumberMax)
        {
            lightSettings.pointLightOuterRadius += lightReward;
            lightSettings.pointLightInnerRadius += lightReward;
        }
    }
}
