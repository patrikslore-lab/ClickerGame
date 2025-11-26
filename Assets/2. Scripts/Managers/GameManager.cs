using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Gameplay, LevelComplete, Paused, GameOver}

    // Player Config (holds all player data)
    [SerializeField] private PlayerConfig playerConfig;
    
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
        UIManager.Instance.InitializePanels();
    }

    //Gamestate logic handling
    private GameState currentGameState = GameState.Gameplay;
    public GameState CurrentGameState => currentGameState;

    public void SetGameState(GameState newState)
    {
        if (currentGameState == newState)
            return;
            
        currentGameState = newState;
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnGameStateChanged(newState);
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