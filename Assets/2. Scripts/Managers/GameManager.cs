using UnityEngine;
using System;
using GameStateMachine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Player Config (holds all player data)
    [SerializeField] private PlayerConfig playerConfig;

    // State Machine
    public GameStateMachine.StateMachine StateMachine { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize the state machine
        StateMachine = new GameStateMachine.StateMachine(this);
    }

    private void Start()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.InitializePanels();
        }
        else
        {
            Debug.LogError("UIManager.Instance is null in GameManager.Start()!");
        }

        // Initialize to MainMenu state
        StateMachine.Initialize(StateMachine.mainMenuState);
    }

    private void Update()
    {
        // Update the current state
        StateMachine?.Update();
    }

    //---------------------------------------------------
    // State transition helper methods
    //---------------------------------------------------

    public void TransitionToMainMenu()
    {
        StateMachine.TransitionTo(StateMachine.mainMenuState);
    }

    public void TransitionToBase()
    {
        StateMachine.TransitionTo(StateMachine.baseState);
    }

    public void TransitionToLevelGameplay()
    {
        StateMachine.TransitionTo(StateMachine.levelGameplayState);
    }

    public void TransitionToPaused()
    {
        // Store the previous state before pausing
        StateMachine.pausedState.SetPreviousState(StateMachine.CurrentState);
        StateMachine.TransitionTo(StateMachine.pausedState);
    }

    public void ResumeFromPause()
    {
        // Resume to the previous state
        GameStateMachine.IGameState previousState = StateMachine.pausedState.GetPreviousState();
        if (previousState != null)
        {
            StateMachine.TransitionTo(previousState);
        }
    }

    public void TransitionToLevelComplete()
    {
        StateMachine.TransitionTo(StateMachine.levelCompleteState);
    }

    public void TransitionToGameOver()
    {
        StateMachine.TransitionTo(StateMachine.gameOverState);
    }

    public void TransitionToNextLevel()
    {
        playerConfig.currentLevel++;
        TransitionToLevelGameplay();
    }
    public void TransitionToLevelInitialState()
    {
        StateMachine.TransitionTo(StateMachine.levelInitialState);
    }

    //---------------------------------------------------
    // Helper properties for backward compatibility
    //---------------------------------------------------

    public bool IsInMainMenu => StateMachine?.CurrentState is GameStateMachine.MainMenuState;
    public bool IsInBase => StateMachine?.CurrentState is GameStateMachine.BaseState;
    public bool IsInLevelGameplay => StateMachine?.CurrentState is GameStateMachine.LevelGameplayState;
    public bool IsInPaused => StateMachine?.CurrentState is GameStateMachine.PausedState;
    public bool IsInLevelComplete => StateMachine?.CurrentState is GameStateMachine.LevelCompleteState;
    public bool IsInGameOver => StateMachine?.CurrentState is GameStateMachine.GameOverState;
    public bool IsInLevelInitialState => StateMachine?.CurrentState is GameStateMachine.LevelInitialState;

    //---------------------------------------------------
    // Data accessors
    //---------------------------------------------------

    public PlayerConfig GetPlayerConfig()
    {
        return playerConfig;
    }

    //---------------------------------------------------
    // Legacy methods (for UI buttons during migration)
    //---------------------------------------------------

    public void StartNewGame()
    {
        playerConfig.currentLevel = 1;
    }
}
