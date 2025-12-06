// LevelGameplayState.cs
using UnityEngine;

namespace GameStateMachine
{
    public class LevelGameplayState : IGameState
    {
        private GameManager gameManager;
        private bool isSubscribed = false;

        public LevelGameplayState(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void Enter()
        {
            Debug.Log("Entering LevelGameplay State");
            
            // Ensure UI is fully visible (UIManager owns the details)
            UIManager.Instance?.ShowGameplayUI();
            
            // Subscribe to gameplay events
            SubscribeToEvents();
            
            // Start combat
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.startCombatSession();
            }
            else
            {
                Debug.LogError("LevelGameplayState: LevelManager.Instance is null!");
            }
        }

        private void SubscribeToEvents()
        {
            if (EventManager.Instance == null || isSubscribed) return;
            
            EventManager.Instance.OnLightDepleted += HandleGameOver;
            // Note: Level completion handled by DoorController.OnDoorBreakAnimationComplete()
            isSubscribed = true;
        }

        private void UnsubscribeFromEvents()
        {
            if (EventManager.Instance == null || !isSubscribed) return;
            
            EventManager.Instance.OnLightDepleted -= HandleGameOver;
            isSubscribed = false;
        }

        private void HandleGameOver()
        {
            Debug.Log("Light depleted - game over");
            gameManager.TransitionToGameOver();
        }

        public void Update()
        {
            // Combat systems run independently:
            // - RoomManager: wave spawning via coroutines
            // - Enemy scripts: behavior and attacks  
            // - LightManager: health tracking (event-driven)
            // - InputManager: clicks and pause
            // - DoorController: triggers level complete on final break
        }

        public void Exit()
        {
            Debug.Log("Exiting LevelGameplay State");
            UnsubscribeFromEvents();
        }
    }
}