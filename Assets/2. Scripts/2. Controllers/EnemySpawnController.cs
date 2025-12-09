// EnemySpawnController.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

/// <summary>
/// Handles enemy wave spawning for a combat session.
/// Controlled by LevelManager.
/// </summary>
public class EnemySpawnController : MonoBehaviour
{
    private RoomConfig roomConfig;
    private Coroutine waveCoroutine;
    private bool isPaused = false;
    public bool IsPaused => isPaused;
    [SerializeField] private GameObject corePrefab;

    private Vector2 playerPosition = new Vector2(0, -7);

    public void StartWaves(RoomConfig config)
    {
        roomConfig = config;
        isPaused = false;
        waveCoroutine = StartCoroutine(SpawnWaves());
    }

    public void StopWaves()
    {
        if (waveCoroutine != null)
        {
            StopCoroutine(waveCoroutine);
            waveCoroutine = null;
        }
        isPaused = false;
    }

    public void Pause() => isPaused = true;
    public void Resume() => isPaused = false;

    private IEnumerator SpawnWaves()
    {
        Debug.Log($"Starting waves. Spawn entries: {roomConfig.EnemySpawns.Count}");

        for (int i = 0; i < roomConfig.EnemySpawns.Count; i++)
        {
            RoomConfig.EnemySpawn spawn = roomConfig.EnemySpawns[i];

            if (spawn.enemyPrefab == null)
            {
                Debug.LogWarning($"Spawn entry {i} has null prefab, skipping");
                continue;
            }

            List<Enemy> spawnGroup = new List<Enemy>();

            for (int j = 0; j < spawn.spawnCount; j++)
            {
                Vector3 position = CalculateSpawnPosition();
                GameObject enemyObj = Instantiate(spawn.enemyPrefab, position, Quaternion.identity);

                if (spawn.doorBreakOnDefeat != RoomConfig.DoorBreakTrigger.None)
                {
                    Enemy enemy = enemyObj.GetComponent<Enemy>();
                    if (enemy != null) spawnGroup.Add(enemy);
                }

                if (j < spawn.spawnCount - 1 && spawn.delayBetweenEnemies > 0)
                {
                    yield return WaitWithPause(spawn.delayBetweenEnemies);
                }
            }

            if (spawn.doorBreakOnDefeat != RoomConfig.DoorBreakTrigger.None && spawnGroup.Count > 0)
            {
                StartCoroutine(MonitorForDoorBreak(spawnGroup, spawn.doorBreakOnDefeat));
            }

            if (i < roomConfig.EnemySpawns.Count - 1 && spawn.delayBeforeNext > 0)
            {
                yield return WaitWithPause(spawn.delayBeforeNext);
            }
        }

        while (EnemyRegistry.Instance.ActiveEnemyCount > 0)
        {
            yield return new WaitForSeconds(0.05f);
        }

        Debug.Log("All enemies defeated");
        EventManager.Instance?.TriggerAllEnemiesDefeated();
    }

    private IEnumerator MonitorForDoorBreak(List<Enemy> enemies, RoomConfig.DoorBreakTrigger trigger)
    {
        while (true)
        {
            enemies.RemoveAll(e => e == null);

            if (enemies.Count == 0)
            {
                TriggerDoorBreak(trigger);
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void TriggerDoorBreak(RoomConfig.DoorBreakTrigger trigger)
    {
        Debug.Log($"Door break: {trigger}");

        switch (trigger)
        {
            case RoomConfig.DoorBreakTrigger.Break1:
                EventManager.Instance?.DoorBreak1();
                break;
            case RoomConfig.DoorBreakTrigger.Break2:
                EventManager.Instance?.DoorBreak2();
                break;
            case RoomConfig.DoorBreakTrigger.Break3:
                EventManager.Instance?.DoorBreak3();
                break;
        }
    }

    private Vector3 CalculateSpawnPosition()
    {
        return new Vector3(
            Random.Range(roomConfig.MinX, roomConfig.MaxX),
            Random.Range(roomConfig.MinY, roomConfig.MaxY),
            roomConfig.SpawnZ
        );
    }

    private IEnumerator WaitWithPause(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (!isPaused) elapsed += Time.deltaTime;
            yield return null;
        }
    }

    //==============================================
    //GAME OVER SEQUENCE INSTANTIATOR/MOVER
    //==============================================
    private List<GameObject> gameOverEnemies = new List<GameObject>();
    public IEnumerator SpawnGameOverWave()
    {
        gameOverEnemies.Clear();
        for (int i = 0; i < 10; i++)
        {
            // Spawn enemy
            Instantiate(corePrefab, CalculateSpawnPosition(), Quaternion.identity);
            gameOverEnemies.Add(corePrefab);
            yield return new WaitForSeconds(0.1f);
        }
    }


    public IEnumerable ConvergeOnPlayer(Vector2 playerPosition, float duration = 2f)
    {
    // Store starting positions
    Vector2[] startPositions = new Vector2[gameOverEnemies.Count];
    Vector2[] endPositions = new Vector2[gameOverEnemies.Count];
    
    for (int i = 0; i < gameOverEnemies.Count; i++)
    {
        if (gameOverEnemies[i] == null) continue;
        
        startPositions[i] = gameOverEnemies[i].transform.position;
        
        // Calculate position 1 unit shy of target
        Vector2 direction = (playerPosition - startPositions[i]).normalized;
        endPositions[i] = playerPosition - direction * 1f;
    }
    
    // Move all enemies simultaneously
    float elapsed = 0f;
    
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;
        
        for (int i = 0; i < gameOverEnemies.Count; i++)
        {
            if (gameOverEnemies[i] == null) continue;
            
            gameOverEnemies[i].transform.position = Vector3.Lerp(
                startPositions[i], 
                endPositions[i], 
                t
            );
        }
        
        yield return null;
    }
    }
}
