using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LooterAbility : BaseAbility
{
    private enum LooterState
    {
        Inactive,
        Active,
        OnCooldown
    }
    private LooterState currentState = LooterState.Inactive;
    private float activeTimer = 0f;

    public bool IsActive => currentState == LooterState.Active;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        // Input handling
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!CanUseAbility())
            {
                Debug.Log("Looter ability on cooldown!");
                return;
            }

            if (currentState == LooterState.Inactive)
            {
                ActivateLooter();
            }
            else if (currentState == LooterState.Active)
            {
                DeactivateLooter();
            }
        }

        // Handle auto-deactivation after max active time
        if (currentState == LooterState.Active)
        {
            activeTimer += Time.deltaTime;
            if (activeTimer >= playerConfig.looterMaxActiveTime)
            {
                DeactivateLooter();
            }
        }

        // Auto-collect wood loot when active
        if (currentState == LooterState.Active && !juneCharacter.IsPerformingAbility)
        {
            Loot[] allLoot = FindObjectsByType<Loot>(FindObjectsSortMode.None);
            Loot closestWood = FindClosestWoodLoot(allLoot);

            if (closestWood != null)
            {
                StartCoroutine(PerformLootSequence(closestWood));
            }
        }
    }

    private void ActivateLooter()
    {
        currentState = LooterState.Active;
        activeTimer = 0f;
        UIManager.Instance.LooterActivate();
        Debug.Log("Looter ability: ACTIVE");
    }

    private void DeactivateLooter()
    {
        currentState = LooterState.Inactive;
        base.StartCooldown();
        UIManager.Instance.LooterOnCooldown();
        Debug.Log("Looter deactivated - COOLDOWN started");
    }

    private IEnumerator PerformLootSequence(Loot targetLoot)
    {
        // Take control of June
        juneCharacter.StartAbilityControl();

        // Fly to loot item
        float flyDuration = Vector3.Distance(juneCharacter.JuneInstance.transform.position, targetLoot.transform.position) / playerConfig.juneMoveSpeed;
        yield return juneCharacter.MoveJuneToPosition(targetLoot.transform.position, flyDuration);

        // Collect the loot
        LootManager.Instance.Collect(targetLoot);
        Debug.Log($"June collected: {targetLoot.lootType}");

        // Return home (also releases control back to JuneCharacter)
        yield return juneCharacter.ReturnJuneHome();
    }

    private Loot FindClosestWoodLoot(Loot[] allLoot)
    {
        Loot closestWood = null;
        float closestDistance = float.MaxValue;

        foreach (Loot loot in allLoot)
        {
            if (loot.lootType == LootType.Wood)
            {
                float distance = Vector3.Distance(juneCharacter.JuneInstance.transform.position, loot.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestWood = loot;
                }
            }
        }

        return closestWood;
    }
}
