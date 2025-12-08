// AbilityController.cs
using UnityEngine;

/// <summary>
/// Controller for ability system - handles cooldowns, input, and ability execution.
/// PlayerManager orchestrates, AbilityController actions.
/// </summary>
[RequireComponent(typeof(RicochetAbility))]
[RequireComponent(typeof(LooterAbility))]
[RequireComponent(typeof(ProtectorAbility))]
public class AbilityController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private KeyCode ricochetKey = KeyCode.R;
    [SerializeField] private KeyCode looterKey = KeyCode.E;
    [SerializeField] private KeyCode protectorKey = KeyCode.W;

    // Cached ability references
    private RicochetAbility ricochet;
    public bool ricochetLocked = true;
    private LooterAbility looter;
    public bool looterLocked = true;
    private ProtectorAbility protector;
    public bool protectorLocked = true;
    // State
    private PlayerConfig playerConfig;
    private IAbility activeAbility;
    private float cooldownTimer;
    private bool isOnCooldown;
    public enum abilityUnlocked
    {
        abilityUnlocked,
        abilityLocked
    }

    // Public properties
    public bool IsOnCooldown => isOnCooldown;
    public float CooldownRemaining => isOnCooldown ? (playerConfig.juneCooldown - cooldownTimer) : 0f;

    private void Awake()
    {
        // Get ability components from this GameObject
        ricochet = GetComponent<RicochetAbility>();
        looter = GetComponent<LooterAbility>();
        protector = GetComponent<ProtectorAbility>();

        if (ricochet == null) Debug.LogError("AbilityController: RicochetAbility component not found!");
        if (looter == null) Debug.LogError("AbilityController: LooterAbility component not found!");
        if (protector == null) Debug.LogError("AbilityController: ProtectorAbility component not found!");
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            playerConfig = GameManager.Instance.GetPlayerConfig();
        }
        else
        {
            Debug.LogError("GameManager.Instance is null in AbilityController.Start()!");
        }
    }

    private void Update()
    {
        UpdateCooldown();
        HandleInput();
    }

    //===========================================
    // COOLDOWN MANAGEMENT
    //===========================================

    private void UpdateCooldown()
    {
        if (!isOnCooldown) return;

        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= playerConfig.juneCooldown)
        {
            isOnCooldown = false;
            cooldownTimer = 0f;
            UIManager.Instance?.RicochetAvailable();
            UIManager.Instance?.LooterAvailable();
            UIManager.Instance?.ProtectorAvailable();
            Debug.Log("Ability cooldown complete - all abilities available");
        }
    }

    private void StartCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = 0f;
        Debug.Log($"Ability cooldown started: {playerConfig.juneCooldown}s");
    }

    //===========================================
    // INPUT HANDLING
    //===========================================

    private void HandleInput()
    {   
        if (Input.GetKeyDown(ricochetKey)) TryActivateAbility(ricochet);
        if (Input.GetKeyDown(looterKey)) TryActivateAbility(looter);
        if (Input.GetKeyDown(protectorKey)) TryActivateAbility(protector);
    }

    /// <summary>
    /// Attempts to activate an ability. Handles toggle-off if already active.
    /// </summary>
    public void TryActivateAbility(IAbility ability)
    {
        if (ability == null) return;

        // Deactivate if already active (toggle off)
        if ((Object)activeAbility == (Object)ability)
        {
            DeactivateCurrentAbility();
            return;
        }

        // Can't activate if on cooldown
        if (isOnCooldown)
        {
            Debug.Log("Abilities on cooldown!");
            return;
        }

        // Can't activate if another ability is active
        if (activeAbility != null)
        {
            Debug.Log("Another ability is active!");
            return;
        }

        // Activate the ability
        ActivateAbility(ability);
    }

    private void ActivateAbility(IAbility ability)
    {
        activeAbility = ability;
        ability.Activate();
        Debug.Log($"{ability.GetType().Name} activated");
    }

    private void DeactivateCurrentAbility()
    {
        if (activeAbility == null) return;

        activeAbility.Deactivate();
        activeAbility = null;
        StartCooldown();
    }

    //===========================================
    // PUBLIC API - ABILITY EVENTS
    //===========================================

    /// <summary>
    /// Called by abilities when they finish (expired or manually deactivated).
    /// </summary>
    public void OnAbilityFinished()
    {
        if (activeAbility == null) return;

        Debug.Log($"{activeAbility.GetType().Name} finished");
        activeAbility = null;
        StartCooldown();
    }

    /// <summary>
    /// Called by InputManager when an enemy is hit - routes to active ability if applicable.
    /// </summary>
    public void OnEnemyHit(Enemy enemy)
    {
        if ((Object)activeAbility == ricochet)
        {
            ricochet.OnEnemyHit(enemy);
        }
    }
}
