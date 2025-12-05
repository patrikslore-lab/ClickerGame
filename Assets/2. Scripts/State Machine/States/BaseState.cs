using UnityEngine;

namespace GameStateMachine
{
    public class BaseState : IGameState
    {
        private GameManager gameManager;

        public BaseState(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void Enter()
        {
            Debug.Log("Entering Base State");

            // Show base area UI
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HideAllPanels();
                UIManager.Instance.basePanel?.SetActive(true);
                UIManager.Instance.gameplayPanel?.SetActive(true);
            }

            // Load base area via LevelManager
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.LoadBaseArea();
            }

            // Ensure time is running
            Time.timeScale = 1f;
        }

        public void Update()
        {
            // ========================================
            // ACTIVE SYSTEMS (running during this state)
            // ========================================
            // None - base area is primarily for UI navigation

            // ========================================
            // PASSIVE SYSTEMS (independent Update loops)
            // ========================================
            // - InputManager: Handles pause (ESC key), mouse clicks for base interactions
            // - UIManager: Resource counters (wood, cores), button clicks

            // ========================================
            // EVENT-DRIVEN SYSTEMS
            // ========================================
            // None active - no enemies or combat events in base
        }

        public void Exit()
        {
            Debug.Log("Exiting Base State");
        }
    }
}
