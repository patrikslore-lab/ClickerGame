// LooterAbility.cs
using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// Looter ability - when active, June automatically collects nearby wood loot.
/// </summary>
public class LooterAbility : BaseAbility, IAbility
{
    private PlayerConfig playerConfig;
    private JuneCharacter june;
    private AbilityController abilityController;
    private float timer;
    private bool isActive;
    public bool IsUnlocked => playerConfig != null && playerConfig.looterUnlocked;

    private void Awake()
    {
        abilityController = GetComponent<AbilityController>();
    }

    private void Start()
    {
        playerConfig = GameManager.Instance.GetPlayerConfig();
    }

    private void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;
        if (timer >= playerConfig.looterMaxActiveTime)
        {
            Deactivate();
            abilityController?.OnAbilityFinished();
            return;
        }

        // Auto-collect when June is free
        if (june != null && !june.IsPerformingAbility)
        {
            Loot closestWood = FindClosestWoodLoot();
            if (closestWood != null)
            {
                StartCoroutine(CollectLoot(closestWood));
            }
        }
    }
    //===========================================
    // IAbility IMPLEMENTATION
    //===========================================

    public void Activate()
    {
        june = PlayerManager.Instance.June;
        isActive = true;
        timer = 0f;
        UIManager.Instance?.LooterActivate();
        Debug.Log("Looter: ACTIVE - auto-collecting wood");
    }

    public void Deactivate()
    {
        isActive = false;
        UIManager.Instance?.LooterOnCooldown();
        Debug.Log("Looter: DEACTIVATED");
    }

    //===========================================
    // LOOTER LOGIC
    //===========================================
    private IEnumerator CollectLoot(Loot loot)
    {
        june.StartAbilityControl();

        float flyDuration = CalculateFlightDuration(june.transform.position, loot.transform.position);
        yield return june.MoveJuneToPosition(loot.transform.position, flyDuration);

        if (loot != null && !loot.IsCollected)
        {
            LevelManager.Instance?.CollectLoot(loot);
            Debug.Log("Looter collected wood");
        }

        yield return june.ReturnJuneHome();
    }

    private Loot FindClosestWoodLoot()
    {
        return FindObjectsByType<Loot>(FindObjectsSortMode.None)
            .Where(l => l.lootType == LootType.Wood && !l.IsCollected)
            .OrderBy(l => Vector3.Distance(june.transform.position, l.transform.position))
            .FirstOrDefault();
    }

    private float CalculateFlightDuration(Vector3 from, Vector3 to)
    {
        return Vector3.Distance(from, to) / playerConfig.juneMoveSpeed;
    }
}
