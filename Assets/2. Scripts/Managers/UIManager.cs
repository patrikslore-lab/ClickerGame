using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject levelCompletionPanel;
    [SerializeField] private GameObject pauseMenuPanel;

    [SerializeField] private GameObject gameOverPanel;

    [SerializeField] private TextMeshProUGUI woodCountTextBox;

    [SerializeField] private TextMeshProUGUI coreCountTextBox;

    [SerializeField] private Image RicochetOnImage;
    [SerializeField] private Image RicochetCooldownImage;
    [SerializeField] private Image RicochetAvailableImage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayerConfig playerConfig = GameManager.Instance.GetPlayerConfig();
        UpdateWoodCountUI(playerConfig.wood);
        UpdateCoreCountUI(playerConfig.corePieces);
    }

    public void InitializePanels()
    {
        // Set initial state to Gameplay
        OnGameStateChanged(GameManager.GameState.Gameplay);
    }
    
    public void OnGameStateChanged(GameManager.GameState newState)
    {
        Debug.Log($"Game State Changed to: {newState}");
        // Hide all panels first
        gameplayPanel?.SetActive(false);
        levelCompletionPanel?.SetActive(false);
        pauseMenuPanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        
        // Show the appropriate panel(s)
        switch (newState)
        {
            case GameManager.GameState.Gameplay:
                gameplayPanel?.SetActive(true);
                Time.timeScale = 1f; // Resume game
                break;
                
            case GameManager.GameState.LevelComplete:
                gameplayPanel?.SetActive(true); // Keep gameplay UI visible
                levelCompletionPanel?.SetActive(true);
                Time.timeScale = 0f; // Pause game
                break;
                
            case GameManager.GameState.Paused:
                gameplayPanel?.SetActive(true); // Keep gameplay UI visible
                pauseMenuPanel?.SetActive(true);
                Time.timeScale = 0f; // Pause game
                break;
            case GameManager.GameState.GameOver:
                gameplayPanel?.SetActive(false);
                pauseMenuPanel?.SetActive(false);
                gameOverPanel?.SetActive(false);
                break;

        }
    }
    public void UpdateWoodCountUI(float totalWood)
    {
        if (woodCountTextBox != null)
            woodCountTextBox.text = $"{totalWood}";
    }
    public void UpdateCoreCountUI(float totalCores)
    {
        if (coreCountTextBox != null)
            coreCountTextBox.text = $"{totalCores}";
    }

    public void RicochetActivate()
    {
        RicochetOnImage.enabled = true;
        RicochetCooldownImage.enabled = false;
        RicochetAvailableImage.enabled = false;
    }

    public void RicochetOnCooldown()
    {
        RicochetOnImage.enabled = false;
        RicochetCooldownImage.enabled = true;
        RicochetAvailableImage.enabled = false;
    }

    public void RicochetAvailable()
    {
        RicochetOnImage.enabled = false;
        RicochetCooldownImage.enabled = false;
        RicochetAvailableImage.enabled = true;
    }
}