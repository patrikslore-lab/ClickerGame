using System;
using UnityEngine;

namespace GameStateMachine
{
    [Serializable]
    public class StateMachine
    {
        public IGameState CurrentState { get; private set; }

        // Reference to all state objects
        public MainMenuState mainMenuState;
        public BaseState baseState;
        public LevelGameplayState levelGameplayState;
        public PausedState pausedState;
        public LevelCompleteState levelCompleteState;
        public GameOverState gameOverState;
        public LevelInitialState levelInitialState;

        // Event to notify other objects of state changes
        public event Action<IGameState> stateChanged;

        // Pass in GameManager reference to constructor
        public StateMachine(GameManager gameManager)
        {
            // Create an instance for each state and pass in GameManager
            this.mainMenuState = new MainMenuState(gameManager);
            this.baseState = new BaseState(gameManager);
            this.levelGameplayState = new LevelGameplayState(gameManager);
            this.pausedState = new PausedState(gameManager);
            this.levelCompleteState = new LevelCompleteState(gameManager);
            this.gameOverState = new GameOverState(gameManager);
            this.levelInitialState = new LevelInitialState(gameManager);
        }

        // Set the starting state
        public void Initialize(IGameState state)
        {
            CurrentState = state;
            state.Enter();

            // Notify other objects that state has changed
            stateChanged?.Invoke(state);
        }

        // Exit current state and enter another
        public void TransitionTo(IGameState nextState)
        {
            CurrentState.Exit();
            CurrentState = nextState;
            nextState.Enter();

            // Notify other objects that state has changed
            stateChanged?.Invoke(nextState);
        }

        // Allow the StateMachine to update the current state
        public void Update()
        {
            if (CurrentState != null)
            {
                CurrentState.Update();
            }
        }
    }
}
