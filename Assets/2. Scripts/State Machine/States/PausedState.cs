using UnityEngine;

namespace GameStateMachine
{
    public class PausedState : IGameState
    {
        private GameManager gameManager;
        private IGameState previousState;

        public PausedState(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void SetPreviousState(IGameState state)
        {
            previousState = state;
        }

        public IGameState GetPreviousState()
        {
            return previousState;
        }

        public void Enter()
        {
            Debug.Log("Entering Paused State");

            // Show pause menu overlay
            if (UIManager.Instance != null)
            {
                // Don't hide all panels - keep current gameplay visible
                UIManager.Instance.gameplayPanel?.SetActive(true);

                // Show base panel if we were in base state
                if (previousState is BaseState)
                {
                    UIManager.Instance.basePanel?.SetActive(true);
                }

                UIManager.Instance.pauseMenuPanel?.SetActive(true);
            }

            // Keep time running (pause is visual only)
            Time.timeScale = 0f;
        }

        public void Update()
        {
            // ========================================
            // ACTIVE SYSTEMS (running during this state)
            // ========================================
            // None - pause freezes gameplay temporarily

            // ========================================
            // PASSIVE SYSTEMS (independent Update loops)
            // ========================================
            // - InputManager: Listens for ESC key to resume
            //   → Calls GameManager.ResumeFromPause() which transitions back to previous state
            // - RoomManager: Wave spawning is PAUSED (isPaused flag set)
            // - LightManager: Still runs but gameplay checks prevent updates

            // ========================================
            // EVENT-DRIVEN SYSTEMS
            // ========================================
            // None - combat events don't trigger while paused
        }

        public void Exit()
        {
            Debug.Log("Exiting Paused State");

            // Hide pause menu
            if (UIManager.Instance != null)
            {
                UIManager.Instance.pauseMenuPanel?.SetActive(false);
            }
        }
    }
}
