using UnityEngine;

public abstract class BaseAbility : MonoBehaviour
{
    protected PlayerConfig playerConfig;
    protected JuneCharacter juneCharacter;
    protected CooldownController cooldownController;

    protected virtual void Start()
    {
        playerConfig = GameManager.Instance.GetPlayerConfig();
        juneCharacter = GetComponent<JuneCharacter>();
        cooldownController = GetComponent<CooldownController>();

        if (juneCharacter == null)
        {
            Debug.LogError($"{GetType().Name}: JuneCharacter component not found on same GameObject!");
        }

        if (cooldownController == null)
        {
            Debug.LogError($"{GetType().Name}: CooldownController component not found on same GameObject!");
        }
    }

    protected bool CanUseAbility()
    {
        if (cooldownController != null && cooldownController.IsOnCooldown)
        {
            return false;
        }

        if (juneCharacter != null && juneCharacter.IsPerformingAbility)
        {
            return false;
        }

        return true;
    }

    protected void StartCooldown()
    {
        if (cooldownController != null)
        {
            cooldownController.StartGlobalCooldown();
        }
    }
}
