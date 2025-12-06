// LootSpawnController.cs
using UnityEngine;
using System.Collections;

/// <summary>
/// Handles loot spawning (wood timer, core drops).
/// Controlled by LevelManager.
/// </summary>
public class LootSpawnController : MonoBehaviour
{
    private GameObject woodLootPrefab;
    private GameObject coreLootPrefab;

    private RoomConfig roomConfig;
    private Coroutine woodCoroutine;

    private void Awake()
    {
        // Load prefabs from Resources
        woodLootPrefab = Resources.Load<GameObject>("Prefabs/WoodLoot");
        coreLootPrefab = Resources.Load<GameObject>("Prefabs/CoreLoot");

        if (woodLootPrefab == null)
        {
            Debug.LogWarning("WoodLoot prefab not found in Resources/Prefabs/");
        }

        if (coreLootPrefab == null)
        {
            Debug.LogWarning("CoreLoot prefab not found in Resources/Prefabs/");
        }
    }

    public void StartLootSpawning(RoomConfig config)
    {
        roomConfig = config;

        if (woodLootPrefab != null)
        {
            woodCoroutine = StartCoroutine(SpawnWoodLoop());
        }
    }

    public void StopLootSpawning()
    {
        if (woodCoroutine != null)
        {
            StopCoroutine(woodCoroutine);
            woodCoroutine = null;
        }
    }

    public void SpawnCoreLoot(Vector3 position)
    {
        if (coreLootPrefab == null) return;

        GameObject coreLoot = Instantiate(coreLootPrefab, position, Quaternion.identity);

        CoreLootFlight flight = coreLoot.GetComponent<CoreLootFlight>();
        flight?.FlyToUI();
    }

    private IEnumerator SpawnWoodLoop()
    {
        float frequency = roomConfig.WoodSpawnFrequencySeconds;

        while (true)
        {
            yield return new WaitForSeconds(frequency);

            Vector3 position = CalculateLootPosition();
            Instantiate(woodLootPrefab, position, Quaternion.identity);
        }
    }

    private Vector3 CalculateLootPosition()
    {
        return new Vector3(
            Random.Range(roomConfig.LootMinX, roomConfig.LootMaxX),
            Random.Range(roomConfig.LootMinY, roomConfig.LootMaxY),
            roomConfig.LootSpawnZ
        );
    }
}
