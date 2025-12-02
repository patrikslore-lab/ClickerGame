using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private SpawnManagerScript spawnManager;
    private RoomConfig currentRoomConfig;
    private Coroutine waveCoroutine;
    private bool isPaused = false;
    private bool levelComplete = false;  

    public void Start()
    {
        if (spawnManager == null)
        {
            spawnManager = GetComponent<SpawnManagerScript>();
        }
    }

    public void LoadRoom(int roomNumber)
    {
        levelComplete = false;

        currentRoomConfig = Resources.Load<RoomConfig>($"Rooms/Room_{roomNumber}");

        if (currentRoomConfig == null)
        {
            Debug.LogError($"RoomConfig for Room {roomNumber} not found in Resources/Rooms/");
            return;
        }

        isPaused = false;
        waveCoroutine = StartCoroutine(SpawnWaves(currentRoomConfig));
        spawnManager.WoodSpawningLogic(currentRoomConfig);
    }

    public void LoadDoor(GameObject door)
    {
        Instantiate(door, new Vector2 (0,8.2f) , Quaternion.identity);
    }

    public void KillDoor(GameObject door)
    {
        
    }

    private IEnumerator SpawnWaves(RoomConfig roomConfig)
    {
        Debug.Log($"Starting enemy spawns for Room {roomConfig.RoomNumber}. Total spawn entries: {roomConfig.EnemySpawns.Count}");
        List<RoomConfig.EnemySpawn> enemySpawns = roomConfig.EnemySpawns;

        for (int spawnIndex = 0; spawnIndex < enemySpawns.Count; spawnIndex++)
        {
            RoomConfig.EnemySpawn spawn = enemySpawns[spawnIndex];

            if (spawn.enemyPrefab == null)
            {
                Debug.LogWarning($"Spawn entry {spawnIndex} has null enemy prefab, skipping");
                continue;
            }

            Debug.Log($"Spawning {spawn.spawnCount}x {spawn.enemyPrefab.name}");

            // Track enemies in this spawn group for door break
            List<Enemy> spawnGroupEnemies = new List<Enemy>();

            // Spawn each enemy with delay between them
            for (int i = 0; i < spawn.spawnCount; i++)
            {
                Vector3 spawnPos = CalculatePosition();
                GameObject enemyObj = Instantiate(spawn.enemyPrefab, spawnPos, Quaternion.identity);

                // Track this enemy if door break is configured
                if (spawn.doorBreakOnDefeat != RoomConfig.DoorBreakTrigger.None)
                {
                    Enemy enemy = enemyObj.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        spawnGroupEnemies.Add(enemy);
                    }
                }

                // Wait between individual enemies in this group
                if (i < spawn.spawnCount - 1 && spawn.delayBetweenEnemies > 0)
                {
                    yield return WaitForPause(spawn.delayBetweenEnemies);
                }
            }

            // If this spawn group has a door break trigger, start monitoring
            if (spawn.doorBreakOnDefeat != RoomConfig.DoorBreakTrigger.None && spawnGroupEnemies.Count > 0)
            {
                StartCoroutine(MonitorSpawnGroupForDoorBreak(spawnGroupEnemies, spawn.doorBreakOnDefeat));
            }

            // Wait before next spawn entry
            if (spawnIndex < enemySpawns.Count - 1 && spawn.delayBeforeNext > 0)
            {
                yield return WaitForPause(spawn.delayBeforeNext);
            }
        }

        // Wait for all enemies to be destroyed
        // Note: Level completion is now triggered by the door break animation event
        // via DoorController.OnDoorBreakAnimationComplete()
        while (!levelComplete)
        {
            Enemy[] remainingEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

            if (remainingEnemies.Length == 0)
            {
                levelComplete = true;
                Debug.Log("All enemies defeated - waiting for door break animation to complete");
                break;
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator MonitorSpawnGroupForDoorBreak(List<Enemy> enemies, RoomConfig.DoorBreakTrigger doorBreakTrigger)
    {
        // Wait until all enemies in this spawn group are dead or destroyed
        while (true)
        {
            // Remove null references (destroyed enemies)
            enemies.RemoveAll(e => e == null);

            // Check if all are dead
            bool allDead = true;
            foreach (Enemy enemy in enemies)
            {
                if (!enemy.IsDead())
                {
                    allDead = false;
                    break;
                }
            }

            if (allDead && enemies.Count == 0)
            {
                // All enemies defeated, trigger door break
                TriggerDoorBreak(doorBreakTrigger);
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void TriggerDoorBreak(RoomConfig.DoorBreakTrigger doorBreakTrigger)
    {
        switch (doorBreakTrigger)
        {
            case RoomConfig.DoorBreakTrigger.Break1:
                Debug.Log("Triggering Door Break 1");
                EventManager.Instance.DoorBreak1();
                break;
            case RoomConfig.DoorBreakTrigger.Break2:
                Debug.Log("Triggering Door Break 2");
                EventManager.Instance.DoorBreak2();
                break;
            case RoomConfig.DoorBreakTrigger.Break3:
                Debug.Log("Triggering Door Break 3");
                EventManager.Instance.DoorBreak3();
                break;
        }
    }

    private Vector3 CalculatePosition()
    {
        return spawnManager.CalculateSpawnPosition(currentRoomConfig);
    }

    private IEnumerator WaitForPause(float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            if (!isPaused)
            {
                elapsed += Time.deltaTime;
            }
            yield return null;
        }
    }

    public void PauseWaves() => isPaused = true;
    public void ResumeWaves() => isPaused = false;
    public void StopWaves()
    {
        if (waveCoroutine != null)
        {
            StopCoroutine(waveCoroutine);
            waveCoroutine = null;
        }
        isPaused = false;
    }

    public bool IsPaused => isPaused;
    public RoomConfig CurrentRoomConfig => currentRoomConfig;


    //Eventually needs to live inside game manager - but placed here for now
    private void LevelComplete()
    {
        Debug.Log("Level Complete!");
        GameManager.Instance.SetGameState(GameManager.GameState.LevelComplete);
    }
}