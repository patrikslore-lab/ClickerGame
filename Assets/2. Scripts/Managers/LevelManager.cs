// LevelManager.cs
using UnityEngine;
using System.Linq;

/// <summary>
/// Singleton manager for level lifecycle.
/// Orchestrates controllers for intro, enemies, and loot.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Display")]
    [SerializeField] private SpriteRenderer levelSpriteRenderer;
    [SerializeField] private GameObject levelGameObject;

    [Header("Controllers")]
    [SerializeField] private LevelIntroController introController;
    [SerializeField] private EnemySpawnController enemySpawnController;
    [SerializeField] private LootSpawnController lootSpawnController;

    private RoomConfig currentRoomConfig;
    private PlayerConfig playerConfig;

    public RoomConfig CurrentRoomConfig => currentRoomConfig;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        playerConfig = GameManager.Instance.GetPlayerConfig();
    }

    //===========================================
    // PUBLIC API
    //===========================================

    public void LoadLevel(int levelNumber)
    {
        currentRoomConfig = Resources.Load<RoomConfig>($"Rooms/Room_{levelNumber}");

        if (currentRoomConfig == null)
        {
            Debug.LogError($"Could not load RoomConfig for level {levelNumber}");
            return;
        }

        if (levelSpriteRenderer != null && currentRoomConfig.RoomSprite != null)
        {
            levelSpriteRenderer.sprite = currentRoomConfig.RoomSprite;
        }

        if (currentRoomConfig.door != null)
        {
            Instantiate(currentRoomConfig.door, new Vector2(0, 8.2f), Quaternion.identity);
        }

        Debug.Log($"Level {levelNumber} loaded");
    }

    public void LoadBaseArea()
    {
        Debug.Log("Loading Base Area");

        playerConfig.lightHealthCurrent = playerConfig.lightHealthMax;

        RoomConfig baseConfig = Resources.Load<RoomConfig>("Rooms/BASE");

        if (baseConfig != null && levelSpriteRenderer != null && baseConfig.RoomSprite != null)
        {
            levelSpriteRenderer.sprite = baseConfig.RoomSprite;
        }

        StopCombatSession();
        DestroyAllCombatObjects();
    }

    public void StartCombatSession()
    {
        if (currentRoomConfig == null)
        {
            Debug.LogError("No RoomConfig loaded - call LoadLevel first");
            return;
        }

        enemySpawnController?.StartWaves(currentRoomConfig);
        lootSpawnController?.StartLootSpawning(currentRoomConfig);

        Debug.Log("Combat session started");
    }

    public void StopCombatSession()
    {
        enemySpawnController?.StopWaves();
        lootSpawnController?.StopLootSpawning();

        Debug.Log("Combat session stopped");
    }

    public void PauseCombat()
    {
        enemySpawnController?.Pause();
    }

    public void ResumeCombat()
    {
        enemySpawnController?.Resume();
    }

    public void SpawnCoreLoot(Vector3 position)
    {
        lootSpawnController?.SpawnCoreLoot(position);
    }

    public LevelIntroController GetIntroController()
    {
        return introController;
    }

    //===========================================
    // CLEANUP
    //===========================================

    private void DestroyAllCombatObjects()
    {
        int enemyCount = EnemyRegistry.Instance.ActiveEnemyCount;

        foreach (Enemy enemy in EnemyRegistry.Instance.GetAllEnemies().ToList())
        {
            Destroy(enemy.gameObject);
        }
        EnemyRegistry.Instance.Clear();

        Loot[] loot = FindObjectsByType<Loot>(FindObjectsSortMode.None);
        foreach (Loot lootItem in loot)
        {
            Destroy(lootItem.gameObject);
        }

        Debug.Log($"Destroyed {enemyCount} enemies and {loot.Length} loot items");
    }

    public RoomConfig GetCurrentRoomConfig() => currentRoomConfig;
}