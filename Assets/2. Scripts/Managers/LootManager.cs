using UnityEngine;

public class LootManager : MonoBehaviour
{
    public static LootManager Instance { get; private set; }
    private PlayerConfig playerConfig;

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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerConfig = GameManager.Instance.GetPlayerConfig();
        EventManager.Instance.LootClicked += HandleLootCollected;
    }

    private void HandleLootCollected(Loot loot)
    {
        Collect(loot);
    }

    public void Collect(Loot loot)
    {
        if (playerConfig == null)
        {
            Debug.LogError("LootManager: PlayerConfig not found!");
            return;
        }

        // Determine loot type and handle collection
        switch (loot.lootType)
        {
            case LootType.Wood:
                playerConfig.wood += playerConfig.woodDropAmount;
                Debug.Log($"Collected {playerConfig.woodDropAmount} wood!");
                UIManager.Instance.UpdateWoodCountUI(playerConfig.wood);
                break;

            case LootType.Core:
                playerConfig.corePieces += playerConfig.corePieceAmount;
                Debug.Log($"Collected {playerConfig.corePieceAmount} core pieces!");
                UIManager.Instance.UpdateCoreCountUI(playerConfig.corePieces);
                break;
        }

        // Despawn the loot
        loot.PlayDespawnAnimation();
    }

    private void OnDestroy()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.LootClicked -= HandleLootCollected;
        }
    }
}
