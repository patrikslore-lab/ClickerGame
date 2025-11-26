using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private SpriteRenderer levelSpriteRenderer;
    [SerializeField] private RoomManager roomManager;

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
        // Load initial level from player config
        PlayerConfig playerConfig = GameManager.Instance.GetPlayerConfig();
        LoadLevel(playerConfig.currentLevel);
    }

    public void LoadLevel(int levelNumber)
    {
        string path = $"Assets/5. Rooms/Room_{levelNumber}.asset";
        RoomConfig roomConfig = UnityEditor.AssetDatabase.LoadAssetAtPath<RoomConfig>(path);

        if (roomConfig == null)
        {
            Debug.LogError($"RoomConfig for Level {levelNumber} not found at {path}");
            return;
        }

        // Update sprite
        if (levelSpriteRenderer != null && roomConfig.RoomSprite != null)
        {
            levelSpriteRenderer.sprite = roomConfig.RoomSprite;
        }
        
        // Load the room (enemies, waves, etc)
        roomManager.LoadRoom(levelNumber);
    }

    public void LoadNextLevel()
    {
        PlayerConfig playerConfig = GameManager.Instance.GetPlayerConfig();
        int nextLevel = playerConfig.currentLevel + 1;
        
        // Reset game state
        GameManager.Instance.SetGameState(GameManager.GameState.Gameplay);
    
        // Load the next level
        GameManager.Instance.LoadLevel(nextLevel);
    }
}