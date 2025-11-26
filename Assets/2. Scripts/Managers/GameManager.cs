using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // WHERE the player is (area/scene context)
    public enum GameMode
    {
        MainMenu,
        Base,
        Combat
    }

    // WHAT is happening (gameplay flow state)
    public enum GameState
    {
        Playing,        // Normal gameplay/interaction
        Paused,         // Game paused (ESC pressed)
        LevelComplete,  // Level finished (Combat only)
        GameOver        // Player defeated (Combat only)
    }

    // Player Config (holds all player data)
    [SerializeField] private PlayerConfig playerConfig;

    private GameMode currentGameMode = GameMode.MainMenu;
    private GameState currentGameState = GameState.Playing;

    public GameMode CurrentGameMode => currentGameMode;
    public GameState CurrentGameState => currentGameState;

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
        Debug.Log("GameManager.Start() called");

        UIManager.Instance.InitializePanels();

        // Start in Base area
        // TODO: Change this to MainMenu when you add main menu screen
        Debug.Log("About to call LoadBaseArea()");
        LevelManager.Instance.LoadBaseArea();
        Debug.Log("LoadBaseArea() completed");
    }

    //Gamestate logic handling
    public void SetGameMode(GameMode newMode)
    {
        if (currentGameMode == newMode)
            return;

        currentGameMode = newMode;
        OnGameModeChanged(newMode);
        Debug.Log($"GameMode changed to: {newMode}");
    }

    public void SetGameState(GameState newState)
    {
        if (currentGameState == newState)
            return;

        currentGameState = newState;
        OnGameStateChanged(newState);
        Debug.Log($"GameState changed to: {newState}");
    }

    private void OnGameModeChanged(GameMode newMode)
    {
        // Always reset to Playing state when changing modes
        currentGameState = GameState.Playing;

        // Force UI update even if state didn't change
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnGameModeChanged(newMode);
            UIManager.Instance.OnGameStateChanged(GameState.Playing, newMode);
        }
    }

    private void OnGameStateChanged(GameState newState)
    {
        // Notify UI Manager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnGameStateChanged(newState, currentGameMode);
        }

        // Handle time scale based on state
        switch (newState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
            case GameState.LevelComplete:
            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
        }
    }
    //---------------------------------------------------
    public void LoadLevel(int levelNumber)
    {
        playerConfig.currentLevel = levelNumber;
        LevelManager.Instance.LoadLevel(levelNumber);
    }
    //---------------------------------------------------
    public PlayerConfig GetPlayerConfig()
    {
        return playerConfig;
    }
}