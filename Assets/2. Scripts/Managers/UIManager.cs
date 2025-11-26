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

    [SerializeField] private GameObject upgradePanel;

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
    }

    // Called when GameMode changes (MainMenu, Base, Combat)
    public void OnGameModeChanged(GameManager.GameMode newMode)
    {
        Debug.Log($"UI: GameMode changed to {newMode}");
        // Mode changes are handled by OnGameStateChanged with the mode parameter
    }

    // Called when GameState changes (Playing, Paused, LevelComplete, GameOver)
    public void OnGameStateChanged(GameManager.GameState newState, GameManager.GameMode currentMode)
    {
        Debug.Log($"UI: GameState changed to {newState} in mode {currentMode}");

        // Hide all panels first
        HideAllPanels();

        // Show appropriate panels based on BOTH mode and state
        if (currentMode == GameManager.GameMode.MainMenu)
        {
            mainMenuPanel?.SetActive(true);
            Debug.Log("Showing mainMenuPanel");
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
}