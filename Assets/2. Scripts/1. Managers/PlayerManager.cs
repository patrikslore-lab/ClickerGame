// PlayerManager.cs
using UnityEngine;

/// <summary>
/// Manages player systems: upgrades, abilities, resources.
/// Orchestrates high-level logic, delegates to controllers.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Header("Controllers")]
    [SerializeField] private AbilityController abilityController;
    [SerializeField] private UpgradeController upgradeController;
    [SerializeField] private DoggySpawnController doggySpawnController;
    [SerializeField] private float doggyCost;

    public enum abilityUnlocked
    {
        abilityUnlocked,
        abilityLocked
    }

    [Header("June")]
    [SerializeField] private GameObject juneGameObject;  // The June GameObject in scene

    // Cached references
    private JuneCharacter june;

    // State
    private PlayerConfig playerConfig;

    // Public
    public bool IsOnCooldown => abilityController != null && abilityController.IsOnCooldown;
    public float CooldownRemaining => abilityController != null ? abilityController.CooldownRemaining : 0f;
    public JuneCharacter June => june;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Get JuneCharacter component from the assigned GameObject
        if (juneGameObject != null)
        {
            june = juneGameObject.GetComponent<JuneCharacter>();
            if (june == null)
            {
                Debug.LogError("PlayerManager: juneGameObject missing JuneCharacter component!");
            }
        }
        else
        {
            Debug.LogError("PlayerManager: juneGameObject not assigned!");
        }

        // Validate AbilityController reference
        if (abilityController == null)
        {
            Debug.LogError("PlayerManager: AbilityController not assigned!");
        }
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            playerConfig = GameManager.Instance.GetPlayerConfig();
        }
        else
        {
            Debug.LogError("GameManager.Instance is null in PlayerManager.Start()!");
        }
    }

    //===========================================
    // PUBLIC API - ABILITIES (Delegate to AbilityController)
    //===========================================

    /// <summary>
    /// Called by abilities when they finish (expired or manually).
    /// Delegates to AbilityController.
    /// </summary>
    public void OnAbilityFinished()
    {
        abilityController?.OnAbilityFinished();
    }

    /// <summary>
    /// Called by InputManager when enemy is hit.
    /// Delegates to AbilityController.
    /// </summary>
    public void OnEnemyHit(Enemy enemy)
    {
        abilityController?.OnEnemyHit(enemy);
    }

    //===========================================
    // PUBLIC API - UPGRADES
    //===========================================

    public void BuyDoggy()
    {
        if (playerConfig.wood >= doggyCost)
        {
            upgradeController.PayDoggy(doggyCost);
            doggySpawnController.SpawnBaseDoggy();
            UIManager.Instance?.UpdateWoodCountUI(playerConfig.wood);
        }
        else
        {
            Debug.Log("Not enough wood for doggy!");
        }
    }
}
