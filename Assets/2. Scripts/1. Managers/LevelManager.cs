// LevelManager.cs
using UnityEngine;
using System.Linq;
using System.Collections;
using Unity.VisualScripting;

/// <summary>
/// Singleton manager for level lifecycle.
/// Orchestrates controllers for intro, enemies, loot, and lantern.
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
    [SerializeField] private LootController lootController;
    [SerializeField] private LanternController lanternController;
    [SerializeField] private DoorController doorController;
    [SerializeField] private GameOverSequenceController gameOverSequenceController;

    [SerializeField] Vector2 playerPosition = new Vector2 (0, -7);

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
    // PUBLIC API - LEVEL LIFECYCLE
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

        doorController.InstantiateDoor();
        lanternController?.SpawnLantern(new Vector2 (0,-15));

        Debug.Log($"Level {levelNumber} loaded");
    }

    public void LoadBaseArea()
    {
        Debug.Log("Loading Base Area");

        playerConfig.lightHealthCurrent = playerConfig.lightHealthMax;

        RoomConfig baseConfig = Resources.Load<RoomConfig>("Rooms/BASE");

        playerConfig.currentLevel = 0;

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
        lootController?.StartSpawning(currentRoomConfig);
        lanternController?.StartGameplay();

        Debug.Log("Combat session started");
    }

    public void StopCombatSession()
    {
        enemySpawnController?.StopWaves();
        lootController?.StopSpawning();
        lanternController?.StopGameplay();

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

    //===========================================
    // PUBLIC API - CORE HIT (delegates to controllers)
    //===========================================

    /// <summary>
    /// Handle core hit - delegates to relevant controllers.
    /// Called by InputManager when a core is clicked.
    /// </summary>
    public void HandleCoreHit(Enemy enemy, float timeTaken)
    {
        // Loot decision (spawn core loot if fast enough)
        lootController?.HandleCoreHit(enemy, timeTaken);

        // Light reward (add light health)
        lanternController?.HandleCoreHit(enemy);
    }

    //===========================================
    // PUBLIC API - LOOT (delegates to LootController)
    //===========================================

    /// <summary>
    /// Collect loot and add resources to player
    /// </summary>
    public void CollectLoot(Loot loot)
    {
        lootController?.Collect(loot);
    }

    /// <summary>
    /// Pay resources for purchases/upgrades.
    /// Returns true if payment successful, false if insufficient resources.
    /// </summary>
    public bool PayResource(LootType resourceType, float amount)
    {
        return lootController?.Pay(resourceType, amount) ?? false;
    }

    //===========================================
    // PUBLIC API - OTHER
    //===========================================

    public LevelIntroController GetIntroController() => introController;
    public GameOverSequenceController GetGameOverSequenceController() => gameOverSequenceController;
    public RoomConfig GetCurrentRoomConfig() => currentRoomConfig;

    //===========================================
    // STATE CONTROL
    //===========================================

    //IS LEVEL COMPLETE?

    public void DoorAnimationFinished()
    {
        TransitionToLevelComplete();
    }
    //Called by the DOOR CONTROLLER upon animation finish
    public void TransitionToLevelComplete()
    {          
        GameManager.Instance.TransitionToLevelComplete();
    }

    //Called by the UI Manager Next Level button
    public void CompleteLevel()
    {
        DestroyAllCombatObjects();
        lanternController.DespawnLantern();
        doorController.DestroyDoor();
        GameManager.Instance.TransitionToNextLevel();
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
        enemySpawnController?.DestroyGameOverEnemies();

        Debug.Log($"Destroyed {enemyCount} enemies and {loot.Length} loot items");
    }

    //=====================================================
    //GAMEOVER SEQUENCE ORCHESTRATORS
    //=====================================================

    public void PlayGameOverSequence()
    {
        gameOverSequenceController.PlayGameOverSequence();
    }
    public IEnumerator SpawnGameOverEnemies()
    {
        yield return enemySpawnController?.SpawnGameOverWave();
    }

    public IEnumerator ConvergeOnPlayer()
    {
        yield return enemySpawnController.ConvergeOnPlayer(playerPosition, 2);
    }
}
