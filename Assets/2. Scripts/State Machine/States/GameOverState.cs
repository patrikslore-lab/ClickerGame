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
            if (UIManager.Instance != null)
    {
            UIManager.Instance.HideAllPanels();

            // Don't show gameOverPanel here - sequence will do it
    }

            LevelManager.Instance.PlayGameOverSequence();
        
        }

        public void Update()
        {
        }

        public void Exit()
        {
            Debug.Log("Exiting GameOver State");
        }
    }
}
