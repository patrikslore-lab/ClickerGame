// LevelInitialState.cs
using UnityEngine;

namespace GameStateMachine
{
    public class LevelInitialState : IGameState
    {
        private GameManager gameManager;
        private bool isSubscribed = false;

        public LevelInitialState(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void Enter()
        {
            Debug.Log("Entering LevelInitial State");
            
            Time.timeScale = 1f;
            
            // 1. Load the level
            int currentLevel = gameManager.GetPlayerConfig().currentLevel;
            LevelManager.Instance.LoadLevel(currentLevel);
            
            // 2. Prepare UI (UIManager owns the details)
            UIManager.Instance?.PrepareForLevelIntro();
            
            // 3. Subscribe and start intro
            SubscribeToIntroComplete();
            StartIntroOrSkip();
        }

        private void SubscribeToIntroComplete()
        {
            if (EventManager.Instance != null && !isSubscribed)
            {
                EventManager.Instance.OnLevelIntroComplete += HandleIntroComplete;
                isSubscribed = true;
            }
        }

        private void UnsubscribeFromIntroComplete()
        {
            if (EventManager.Instance != null && isSubscribed)
            {
                EventManager.Instance.OnLevelIntroComplete -= HandleIntroComplete;
                isSubscribed = false;
            }
        }

        private void StartIntroOrSkip()
        {
            LevelIntroController introController = LevelManager.Instance?.GetIntroController();

            if (introController != null)
            {
                Debug.Log("Starting level intro sequence");
                introController.PlayIntro();
            }
            else
            {
                Debug.LogWarning("No intro controller - skipping to gameplay");
                TransitionToGameplay();
            }
        }

        private void HandleIntroComplete()
        {
            Debug.Log("Intro complete - transitioning to gameplay");
            TransitionToGameplay();
        }

        private void TransitionToGameplay()
        {
            gameManager.TransitionToLevelGameplay();
        }

        public void Update() { }

        public void Exit()
        {
            Debug.Log("Exiting LevelInitial State");
            UnsubscribeFromIntroComplete();
        }
    }
}