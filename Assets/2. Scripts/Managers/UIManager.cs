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
    [SerializeField] private GameObject basePanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject startGamePanel;
    [SerializeField] private GameObject upgradePanel;

    [SerializeField] private TextMeshProUGUI woodCountTextBox;

    [SerializeField] private TextMeshProUGUI coreCountTextBox;

    [SerializeField] public TextMeshProUGUI juneCooldownTextBox;

    [SerializeField] private Image RicochetOnImage;
    [SerializeField] private Image RicochetCooldownImage;
    [SerializeField] private Image RicochetAvailableImage;

    [SerializeField] private Image LooterOnImage;
    [SerializeField] private Image LooterCooldownImage;
    [SerializeField] private Image LooterAvailableImage;

    private PlayerConfig playerConfig;

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
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is null in UIManager.Start()!");
            return;
        }

        playerConfig = GameManager.Instance.GetPlayerConfig();
        if (playerConfig != null)
        {
            UpdateWoodCountUI(playerConfig.wood);
            UpdateCoreCountUI(playerConfig.corePieces);
        }
        else
        {
            Debug.LogError("PlayerConfig is null! Assign it in GameManager Inspector.");
        }
    }

    void Update()
    {
        if (!CooldownController.Instance.IsOnCooldown)
        {
        juneCooldownTextBox.text = "READY";
        } 
        else
        {
        juneCooldownTextBox.text = $"{CooldownController.Instance.CooldownRemaining:F0}";
        }   
    }

    public void InitializePanels()
    {
        // Hide all panels initially - GameManager will set the correct mode/state
        HideAllPanels();
    }

    private void HideAllPanels()
    {
        gameplayPanel?.SetActive(false);
        levelCompletionPanel?.SetActive(false);
        pauseMenuPanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        basePanel?.SetActive(false);
        mainMenuPanel?.SetActive(false);
        upgradePanel?.SetActive(false);
        startGamePanel?.SetActive(false);
    }

    // Called when GameMode changes (MainMenu, Base, Combat)
    public void OnGameModeChanged(GameManager.GameMode newMode)
    {
        // Mode changes are handled by OnGameStateChanged with the mode parameter
    }

    // Called when GameState changes (Playing, Paused, LevelComplete, GameOver)
    public void OnGameStateChanged(GameManager.GameState newState, GameManager.GameMode currentMode)
    {
        // Hide all panels first
        HideAllPanels();

        // Show appropriate panels based on BOTH mode and state
        if (currentMode == GameManager.GameMode.MainMenu)
        {
            if (mainMenuPanel == null)
            {
                Debug.LogError("mainMenuPanel is NULL! Assign it in UIManager Inspector.");
                return;
            }

            mainMenuPanel.SetActive(true);
            return;
        }

        if (currentMode == GameManager.GameMode.Base)
        {
            switch (newState)
            {
                case GameManager.GameState.Playing:
                    if (basePanel != null)
                    {
                        basePanel.SetActive(true);
                        gameplayPanel?.SetActive(true);
                    }
                    else
                    {
                        Debug.LogError("basePanel is NULL! Assign it in UIManager Inspector.");
                    }
                    break;
                case GameManager.GameState.Paused:
                    basePanel?.SetActive(true);
                    pauseMenuPanel?.SetActive(true);
                    Debug.Log("Showing basePanel + pauseMenuPanel");
                    break;
            }
            return;
        }

        if (currentMode == GameManager.GameMode.Combat)
        {
            switch (newState)
            {
                case GameManager.GameState.Playing:
                    gameplayPanel?.SetActive(true);
                    break;

                case GameManager.GameState.Paused:
                    gameplayPanel?.SetActive(true); // Keep gameplay UI visible
                    pauseMenuPanel?.SetActive(true);
                    break;

                case GameManager.GameState.LevelComplete:
                    gameplayPanel?.SetActive(true); // Keep gameplay UI visible
                    levelCompletionPanel?.SetActive(true);
                    break;

                case GameManager.GameState.GameOver:
                    gameOverPanel?.SetActive(true);
                    break;
            }
        }
    }

    public void Upgrading()
    {
        upgradePanel.SetActive(true);
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

    public void LooterActivate()
    {
        if (LooterOnImage != null)
        {
            LooterOnImage.enabled = true;
            LooterCooldownImage.enabled = false;
            LooterAvailableImage.enabled = false;
        }
    }

    public void LooterOnCooldown()
    {
        if (LooterCooldownImage != null)
        {
            LooterOnImage.enabled = false;
            LooterCooldownImage.enabled = true;
            LooterAvailableImage.enabled = false;
        }
    }

    public void LooterAvailable()
    {
        if (LooterAvailableImage != null)
        {
            LooterOnImage.enabled = false;
            LooterCooldownImage.enabled = false;
            LooterAvailableImage.enabled = true;
        }
    }
}