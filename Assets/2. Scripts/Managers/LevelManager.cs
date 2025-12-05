// LevelManager.cs
using UnityEngine;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private SpriteRenderer levelSpriteRenderer;
    [SerializeField] private GameObject levelGameObject; // ← Add reference to level GameObject
    [SerializeField] private RoomManager roomManager;
    
    private LevelIntroController currentIntroController; // ← Track current intro controller
    
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
        
        // Find the level GameObject if not assigned
        if (levelGameObject == null)
        {
            levelGameObject = levelSpriteRenderer.gameObject;
        }
    }

    public void LoadBaseArea()
    {
        Debug.Log("Loading Base Area");

        playerConfig.lightHealthCurrent = playerConfig.lightHealthMax;

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

        roomManager.StopWaves();
        DestroyAllCombatObjects();
    }

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

    public void LoadLevel(float levelNumber)
    {
        RoomConfig roomConfig = Resources.Load<RoomConfig>($"Rooms/Room_{levelNumber}");

        // Update sprite based on RoomConfig
        if (roomConfig != null && levelSpriteRenderer != null && roomConfig.RoomSprite != null)
        {
            levelSpriteRenderer.sprite = roomConfig.RoomSprite;
            Debug.Log($"Level {levelNumber} background sprite loaded");
        }
        else
        {
            Debug.LogWarning($"Could not load sprite for Level {levelNumber}");
        }

        // Load door if configured
        if (roomConfig.door != null)
        {
            Instantiate(roomConfig.door, new Vector2(0, 8.2f), Quaternion.identity);
            Debug.Log($"Level {levelNumber} door spawned");
        }

        // Get the intro controller from the level GameObject
        currentIntroController = levelGameObject.GetComponent<LevelIntroController>();
        
        if (currentIntroController == null)
        {
            Debug.LogWarning("No LevelIntroController found on level GameObject!");
        }
    }

    public void startCombatSession()
    {
        roomManager.StartCombatSession(playerConfig);
    }

    // ← NEW: Get the intro controller for the current level
    public LevelIntroController GetIntroController()
    {
        return currentIntroController;
    }

    public RoomConfig GetCurrentRoomConfig()
    {
        return roomManager?.CurrentRoomConfig;
    }
}