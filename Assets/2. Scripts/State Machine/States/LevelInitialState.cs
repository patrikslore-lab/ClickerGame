// LevelInitialState.cs
using UnityEngine;

namespace GameStateMachine
{
    public class LevelInitialState : IGameState
    {
        private GameManager gameManager;

        public LevelInitialState(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void Enter()
        {
            Debug.Log("Entering LevelInitial State");

            int currentLevel = gameManager.GetPlayerConfig().currentLevel;

            LevelManager.Instance.LoadLevel(currentLevel);

            // Hide panels as gameplay panel alpha lerp by levelintrocontroller
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideAllPanels();
                UIManager.Instance.gameplayPanel?.SetActive(true);
                UIManager.Instance.gameplayPanel.GetComponent<CanvasGroup>().alpha = 0;  //hiding the panel initially with 0 alpha for it to be revealed by levelintrocontroller
            }

            Time.timeScale = 1f;

            // Subscribe to intro completion
            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnLevelIntroComplete += HandleIntroComplete;
            }

            // Get and start the intro controller
            Debug.Log("LevelInitialState: Attempting to get intro controller...");
            LevelIntroController introController = LevelManager.Instance?.GetIntroController();

            if (introController != null)
            {
                Debug.Log("LevelInitialState: Intro controller found, calling PlayIntro()");
                introController.PlayIntro();
            }

            else
            {
                Debug.LogWarning("LevelInitialState: No intro controller found, skipping to gameplay");
                gameManager.TransitionToLevelGameplay();
            }
        }

        public void Update()
        {
            // Nothing needed here - intro controller handles its own updates
        }

        private void HandleIntroComplete()
        {
            Debug.Log("Intro complete, transitioning to gameplay");
            gameManager.TransitionToLevelGameplay();
        }

        public void Exit()
        {
            Debug.Log("Exiting LevelInitial State");

            if (EventManager.Instance != null)
            {
                EventManager.Instance.OnLevelIntroComplete -= HandleIntroComplete;
            }
        }
    }
}