using UnityEngine;

namespace GameStateMachine
{
    public class GameOverState : IGameState
    {
        private GameManager gameManager;

        public GameOverState(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void Enter()
        {
            Debug.Log("Entering GameOver State");

            GameOverSequenceController GOSController = LevelManager.Instance.GetGameOverSequenceController();
            
            GOSController.PlayGameOverSequence();

            // Show game over UI
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideAllPanels();
                UIManager.Instance.gameOverPanel?.SetActive(true);
            }
        }

        public void Update()
        {
            // ========================================
            // ACTIVE SYSTEMS (running during this state)
            // ========================================
            // None - game is over, waiting for player input

            // ========================================
            // PASSIVE SYSTEMS (independent Update loops)
            // ========================================
            // - UIManager: Game over panel "Return to Base" button
            //   → Calls GameManager.TransitionToBase()

            // ========================================
            // EVENT-DRIVEN SYSTEMS
            // ========================================
            // None - all gameplay stopped

            // NOTE: Time.timeScale = 0f (HARD STOP - nothing updates except UI)
            // This is different from Paused state which keeps time running
        }

        public void Exit()
        {
            Debug.Log("Exiting GameOver State");

            // Restore time scale
            Time.timeScale = 1f;
        }
    }
}
