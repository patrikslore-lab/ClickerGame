// LootController.cs
using UnityEngine;
using System.Collections;

/// <summary>
/// Unified controller for loot spawning, collection, and reward decisions.
/// Controlled by LevelManager.
/// </summary>
public class LootController : MonoBehaviour
{
    private GameObject woodLootPrefab;
    private GameObject coreLootPrefab;

    private RoomConfig roomConfig;
    private PlayerConfig playerConfig;
    private Coroutine woodCoroutine;

    private void Awake()
    {
        woodLootPrefab = Resources.Load<GameObject>("Prefabs/WoodLoot");
        coreLootPrefab = Resources.Load<GameObject>("Prefabs/CoreLoot");

        if (woodLootPrefab == null)
            Debug.LogWarning("LootController: WoodLoot prefab not found in Resources/Prefabs/");

        if (coreLootPrefab == null)
            Debug.LogWarning("LootController: CoreLoot prefab not found in Resources/Prefabs/");
    }

    private void Start()
    {
        playerConfig = GameManager.Instance.GetPlayerConfig();
    }

    //===========================================
    // CORE HIT HANDLING
    //===========================================

    /// <summary>
    /// Handle core hit - decide whether to spawn core loot based on reaction time.
    /// Called by LevelManager.HandleCoreHit()
    /// </summary>
    public void HandleCoreHit(Enemy enemy, float timeTaken)
    {
        if (playerConfig == null) return;

        if (timeTaken <= playerConfig.coreLootMaxReactionTime)
        {
            SpawnCoreLoot(enemy.transform.position);
            Debug.Log($"Core loot awarded! Reaction time: {timeTaken:F0}ms (threshold: {playerConfig.coreLootMaxReactionTime}ms)");
        }
        else
        {
            Debug.Log($"No core loot - too slow. Reaction time: {timeTaken:F0}ms (threshold: {playerConfig.coreLootMaxReactionTime}ms)");
        }
    }

    //===========================================
    // SPAWNING
    //===========================================

    /// <summary>
    /// Start spawning loot for the current room.
    /// Called by LevelManager.StartCombatSession()
    /// </summary>
    public void StartSpawning(RoomConfig config)
    {
        roomConfig = config;

        if (woodLootPrefab != null)
        {
            woodCoroutine = StartCoroutine(SpawnWoodLoop());
        }
    }

    /// <summary>
    /// Stop all loot spawning.
    /// Called by LevelManager.StopCombatSession()
    /// </summary>
    public void StopSpawning()
    {
        if (woodCoroutine != null)
        {
            StopCoroutine(woodCoroutine);
            woodCoroutine = null;
        }
    }

    /// <summary>
    /// Spawn core loot at a specific position (e.g., enemy death location).
    /// </summary>
    private void SpawnCoreLoot(Vector3 position)
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

    //===========================================
    // COLLECTION
    //===========================================

    /// <summary>
    /// Collect loot and add resources to player.
    /// Called by LevelManager.CollectLoot()
    /// </summary>
    public void Collect(Loot loot)
    {
        if (playerConfig == null)
        {
            Debug.LogError("LootController: PlayerConfig not found!");
            return;
        }

        switch (loot.lootType)
        {
            case LootType.Wood:
                playerConfig.wood += playerConfig.woodDropAmount;
                Debug.Log($"Collected {playerConfig.woodDropAmount} wood!");
                UIManager.Instance?.UpdateWoodCountUI(playerConfig.wood);
                break;

            case LootType.Core:
                playerConfig.corePieces += playerConfig.corePieceAmount;
                Debug.Log($"Collected {playerConfig.corePieceAmount} core pieces!");
                UIManager.Instance?.UpdateCoreCountUI(playerConfig.corePieces);
                break;
        }

        loot.PlayDespawnAnimation();
    }

    //===========================================
    // PAYMENT (for future use)
    //===========================================

    /// <summary>
    /// Pay resources for purchases/upgrades.
    /// Returns true if payment successful, false if insufficient resources.
    /// </summary>
    public bool Pay(LootType resourceType, float amount)
    {
        if (playerConfig == null)
        {
            Debug.LogError("LootController: PlayerConfig not found!");
            return false;
        }

        switch (resourceType)
        {
            case LootType.Wood:
                if (playerConfig.wood < amount)
                {
                    Debug.Log($"Not enough wood! Need {amount}, have {playerConfig.wood}");
                    return false;
                }
                playerConfig.wood -= amount;
                UIManager.Instance?.UpdateWoodCountUI(playerConfig.wood);
                Debug.Log($"Paid {amount} wood");
                return true;

            case LootType.Core:
                if (playerConfig.corePieces < amount)
                {
                    Debug.Log($"Not enough cores! Need {amount}, have {playerConfig.corePieces}");
                    return false;
                }
                playerConfig.corePieces -= amount;
                UIManager.Instance?.UpdateCoreCountUI(playerConfig.corePieces);
                Debug.Log($"Paid {amount} core pieces");
                return true;

            default:
                return false;
        }
    }
}
