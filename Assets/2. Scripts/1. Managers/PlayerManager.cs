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
    [SerializeField] private float ricochetCost;


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
    /// Called by abilities when they finish (expired or manually).
    /// Delegates to AbilityController.
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
    //==========================================
    // ABILITY UNLOCKERS
    //===========================================

    //CURRENTLY CONFIGURED TO BE BOUGHT FROM SHOP - IF UNLOCKED FOR FREE NEEDS TO BE CHANGED
    public void UnlockRicochet()
    {
        if (playerConfig.wood >= ricochetCost && playerConfig.ricochetUnlocked == false)
        {
        playerConfig.ricochetUnlocked = true;
        UIManager.Instance?.RefreshAbilityUI();
        upgradeController.PayRicochetPrice(ricochetCost);
        Debug.Log("Ricochet ability unlocked!");
        }
        else if (playerConfig.ricochetUnlocked)
        {
            Debug.Log("Ricochet already unlocked!");
        }
        else
        {
            Debug.Log("Not enough wood to unlock Ricochet!");
        }

    }

    public void UnlockLooter()
    {
        playerConfig.looterUnlocked = true;
        UIManager.Instance?.RefreshAbilityUI();
        Debug.Log("Looter ability unlocked!");
    }

    public void UnlockProtector()
    {
        playerConfig.protectorUnlocked = true;
        UIManager.Instance?.RefreshAbilityUI();
        Debug.Log("Protector ability unlocked!");
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
