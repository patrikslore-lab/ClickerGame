using Mono.Cecil;
using UnityEngine;

namespace GameStateMachine
{
    public class LevelGameplayState : IGameState
    {
        private GameManager gameManager;

        public LevelGameplayState(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }

        public void Enter()
        {
            Debug.Log("entered levelgameplaystate");
            LevelManager.Instance.startCombatSession();
        }

        public void Update()
        {
            // ========================================
            // ACTIVE SYSTEMS (running during this state)
            // ========================================
            // - LightManager: Updates light health, checks for game over (light <= 0)
            // - RoomManager: Wave spawning via coroutines (passive but active in this state)
            // ========================================
            // PASSIVE SYSTEMS (independent Update loops)
            // ========================================
            // - InputManager: Mouse clicks (enemy targeting), ESC for pause
            //   → Checks IsInLevelGameplay to enable crosshair, handle clicks
            // - UIManager: Cooldown text updates (juneCooldownTextBox)
            // - Enemy scripts: Each enemy runs its own Update() for animations, attacks
            // ========================================
            // EVENT-DRIVEN SYSTEMS
            // ========================================
            // - Enemy attacks → EventManager.LightDestruction → LightManager.LightDestruction()
            // - Enemy core clicked → EventManager.CoreHit → LightManager.LightAddition()
            // - Enemy killed → EventManager.EnemyHit → [Various systems]
            // - Protector active → EventManager.ProtectorLightAddition → LightManager.LightAdditionProtector()
            // - Wave complete → RoomManager.MonitorSpawnGroupForDoorBreak → EventManager.DoorBreak3 → DoorController
        }

        public void Exit()
        {
            Debug.Log("Exiting LevelGameplay State");
        }
    }
}
