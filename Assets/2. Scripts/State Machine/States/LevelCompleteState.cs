using UnityEngine;

namespace GameStateMachine
{
    public class LevelCompleteState : IGameState
    {
        private GameManager gameManager;

        public LevelCompleteState(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void Enter()
        {
            Debug.Log("Entering LevelComplete State");

            // Show level completion UI
            if (UIManager.Instance != null)
            {
                // Keep gameplay visible in background
                UIManager.Instance.gameplayPanel?.SetActive(true);
                // Show level completion panel
                UIManager.Instance.levelCompletionPanel?.SetActive(true);
            }

            // Keep time running
            Time.timeScale = 1f;
        }

        public void Update()
        {
            // ========================================
            // ACTIVE SYSTEMS (running during this state)
            // ========================================
            // None - waiting for player decision

            // ========================================
            // PASSIVE SYSTEMS (independent Update loops)
            // ========================================
            // - UIManager: Level completion panel buttons
            //   → "Next Level" button calls GameManager.TransitionToNextLevel()
            //   → "Return to Base" button calls GameManager.TransitionToBase()

            // ========================================
            // EVENT-DRIVEN SYSTEMS
            // ========================================
            // None - gameplay is complete, no combat events

            // NOTE: Gameplay UI remains visible in background for visual continuity
            // Wave spawning and combat systems are inactive
        }

        public void Exit()
        {
            Debug.Log("Exiting LevelComplete State");

            // Hide level completion panel
            if (UIManager.Instance != null)
            {
                UIManager.Instance.levelCompletionPanel?.SetActive(false);
            }
        }
    }
}
