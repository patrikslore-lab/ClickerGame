using UnityEngine;
using System.Linq;

/// <summary>
/// Coordinates level-to-level progression, game mode transitions (Base/Combat),
/// and delegates room-specific logic to RoomManager.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private SpriteRenderer levelSpriteRenderer;
    [SerializeField] private RoomManager roomManager;
    PlayerConfig playerConfig;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        playerConfig = GameManager.Instance.GetPlayerConfig();
    }

    public void LoadBaseArea()
    {
        
        Debug.Log("Loading Base Area");

        // Set game mode to Base
        GameManager.Instance.SetGameMode(GameManager.GameMode.Base);

        playerConfig.lightHealthCurrent = playerConfig.lightHealthMax;

        // Load base room config and apply sprite
        RoomConfig baseRoomConfig = Resources.Load<RoomConfig>("Rooms/BASE");

        if (baseRoomConfig != null && levelSpriteRenderer != null && baseRoomConfig.RoomSprite != null)
        {
            levelSpriteRenderer.sprite = baseRoomConfig.RoomSprite;
            Debug.Log("Base background sprite loaded");
        }
        else
        {
            Debug.LogWarning("BASE.asset not found or missing sprite in Resources/Rooms/");
        }

        // Stop all combat activities
        roomManager.StopWaves();
        DestroyAllCombatObjects();
    }

    private void DestroyAllCombatObjects()
    {
        // Destroy all enemies
        int enemyCount = EnemyRegistry.Instance.ActiveEnemyCount;
        foreach (Enemy enemy in EnemyRegistry.Instance.GetAllEnemies().ToList())
        {
            Destroy(enemy.gameObject);
        }
        EnemyRegistry.Instance.Clear();

        // Destroy all loot
        Loot[] loot = FindObjectsByType<Loot>(FindObjectsSortMode.None);
        foreach (Loot lootItem in loot)
        {
            Destroy(lootItem.gameObject);
        }

        Debug.Log($"Destroyed {enemyCount} enemies and {loot.Length} loot items");
    }

    public void LoadLevel(int levelNumber)
    {
        // Set game mode to Combat when loading a level
        GameManager.Instance.SetGameMode(GameManager.GameMode.Combat);

        RoomConfig roomConfig = Resources.Load<RoomConfig>($"Rooms/Room_{levelNumber}");

        if (roomConfig == null)
        {
            Debug.LogError($"RoomConfig for Level {levelNumber} not found in Resources/Rooms/");
            return;
        }

        // Update sprite
        if (levelSpriteRenderer != null && roomConfig.RoomSprite != null)
        {
            levelSpriteRenderer.sprite = roomConfig.RoomSprite;
        }

        roomManager.LoadDoor(roomConfig.door);

        // Load the room (enemies, waves, etc)
        roomManager.LoadRoom(levelNumber);
    }

    public void LoadNextLevel()
    {
        PlayerConfig playerConfig = GameManager.Instance.GetPlayerConfig();
        int nextLevel = playerConfig.currentLevel + 1;

        // Reset game state to Playing
        GameManager.Instance.SetGameState(GameManager.GameState.Playing);

        // Load the next level
        GameManager.Instance.LoadLevel(nextLevel);
    }

    public RoomConfig GetCurrentRoomConfig()
    {
        return roomManager?.CurrentRoomConfig;
    }
}