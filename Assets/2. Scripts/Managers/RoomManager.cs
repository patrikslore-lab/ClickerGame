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

        if (waveCoroutine != null)
        {
            StopCoroutine(waveCoroutine);
        }

        isPaused = false;
        waveCoroutine = StartCoroutine(SpawnWaves(currentRoomConfig));
        spawnManager.WoodSpawningLogic(currentRoomConfig);
    }

    private IEnumerator SpawnWaves(RoomConfig roomConfig)
    {
        Debug.Log($"Starting wave sequence for Room {roomConfig.RoomNumber}. Total waves: {roomConfig.WaveSequence.Count}");
        List<RoomConfig.WaveReference> waveSequence = roomConfig.WaveSequence;

        for (int waveIndex = 0; waveIndex < waveSequence.Count; waveIndex++)
        {
            RoomConfig.WaveReference waveRef = waveSequence[waveIndex];
            WaveConfig waveConfig = waveRef.waveConfig;
            int timesToRepeat = waveRef.timesToRepeat;
            
            Debug.Log($"Wave {waveIndex}: {waveConfig.WaveName}, Repeat {timesToRepeat} times");

            for (int repeatIndex = 0; repeatIndex < timesToRepeat; repeatIndex++)
            {
                Debug.Log($"  Repeat {repeatIndex + 1}/{timesToRepeat}");
                
                // Spawn all enemy types in this wave (all at once, no delays within wave)
                for (int i = 0; i < waveConfig.EnemiesToSpawn.Count; i++)
                {
                    WaveConfig.EnemySpawnEntry entry = waveConfig.EnemiesToSpawn[i];
                    Debug.Log($"    Spawning {entry.spawnCount} of {entry.enemyConfig.name}");
                    
                    // Spawn all of this enemy type (with variation but no waiting)
                    for (int j = 0; j < entry.spawnCount; j++)
                    {
                        Vector3 spawnPos = CalculatePosition(entry.enemyConfig);
                        spawnManager.SpawnEnemy(entry.enemyConfig, spawnPos, roomConfig);
                    }
                }

                // Wait before next repeat of this wave
                if (repeatIndex < timesToRepeat - 1)
                {
                    yield return WaitForPause(roomConfig.DelayBetweenWaves);
                }
            }

            // Wait before next wave
            if (waveIndex < waveSequence.Count - 1)
            {
                yield return WaitForPause(roomConfig.DelayBetweenWaves);
            }
        }

        // This is the final wave - now wait for all enemies to be destroyed
        bool isFinalWave = true;
        
        // Poll until no enemies remain
        while (isFinalWave && !levelComplete)
        {
            Enemy[] remainingEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            
            if (remainingEnemies.Length == 0)
            {
                levelComplete = true;
                LevelComplete();
                break;
            }
        yield return new WaitForSeconds(0.05f);
        }
    }

    private Vector3 CalculatePosition(EnemyConfig config)
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