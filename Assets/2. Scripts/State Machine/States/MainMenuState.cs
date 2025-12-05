using UnityEngine;

namespace GameStateMachine
{
    public class MainMenuState : IGameState
    {
        private GameManager gameManager;

        public MainMenuState(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void Enter()
        {
            Debug.Log("Entering MainMenu State");

            // Show main menu UI
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideAllPanels();
                UIManager.Instance.mainMenuPanel?.SetActive(true);
            }

            // Enable cursor for main menu
            Cursor.visible = true;

            // Ensure time is running
            Time.timeScale = 1f;
        }

        public void Update()
        {
            // ========================================
            // ACTIVE SYSTEMS (running during this state)
            // ========================================
            // None - main menu is entirely UI-driven
            // ========================================
            // PASSIVE SYSTEMS (independent Update loops)
            // ========================================
            // - InputManager: Checks IsInMainMenu for cursor visibility
            // - UIManager: Button clicks trigger state transitions
            // ========================================
            // EVENT-DRIVEN SYSTEMS
            // ========================================
            // None in main menu
        }

        public void Exit()
        {
            Debug.Log("Exiting MainMenu State");
        }
    }
}
