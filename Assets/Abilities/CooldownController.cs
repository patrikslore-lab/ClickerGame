using TMPro;
using UnityEngine;

public class CooldownController : MonoBehaviour
{
    private static CooldownController instance;
    public static CooldownController Instance => instance;

    private PlayerConfig playerConfig;
    private float cooldownTimer = 0f;
    private bool isOnCooldown = false;
    public bool IsOnCooldown => isOnCooldown;
    public float CooldownRemaining => isOnCooldown ? (playerConfig.juneCooldown - cooldownTimer) : 0f;
    public float CooldownProgress => isOnCooldown ? (cooldownTimer / playerConfig.juneCooldown) : 0f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        playerConfig = GameManager.Instance.GetPlayerConfig();
    }

    private void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer >= playerConfig.juneCooldown)
            {
                EndGlobalCooldown();
            }
        }
    }

    public void StartGlobalCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = 0f;
        Debug.Log($"Global ability cooldown started: {playerConfig.juneCooldown}s");
    }

    private void EndGlobalCooldown()
    {
        isOnCooldown = false;
        cooldownTimer = 0f;
        UIManager.Instance.RicochetAvailable();
        UIManager.Instance.LooterAvailable();
        UIManager.Instance.ProtectorAvailable();
        Debug.Log("Global ability cooldown complete - ALL abilities available");
    }
}
