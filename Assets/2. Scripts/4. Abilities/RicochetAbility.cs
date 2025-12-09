// RicochetAbility.cs
using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// Ricochet ability - when active, clicking an enemy causes June to chain-hit nearby enemies.
/// </summary>
public class RicochetAbility : BaseAbility, IAbility
{
    private PlayerConfig playerConfig;
    private JuneCharacter june;
    private float timer;
    private bool isActive;
    public bool IsUnlocked => playerConfig != null && playerConfig.ricochetUnlocked;

    private void Start()
    {
        playerConfig = GameManager.Instance.GetPlayerConfig();
    }

    private void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;
        if (timer >= playerConfig.ricochetMaxActiveTime)
        {
            Deactivate();
            GetComponent<AbilityController>()?.OnAbilityFinished();
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
        UIManager.Instance?.RicochetActivate();
        Debug.Log("Ricochet: ACTIVE - click enemies to chain hit");
    }

    public void Deactivate()
    {
        isActive = false;
        UIManager.Instance?.RicochetOnCooldown();
        Debug.Log("Ricochet: DEACTIVATED");
    }

    //===========================================
    // RICOCHET LOGIC
    //===========================================

    public void OnEnemyHit(Enemy enemy)
    {
        if (!isActive) return;
        if (june == null || june.IsPerformingAbility) return;

        StartCoroutine(ChainHit(enemy));
    }

    private IEnumerator ChainHit(Enemy hitEnemy)
    {
        june.StartAbilityControl();

        var targets = EnemyRegistry.Instance.GetAllEnemies()
            .Where(e => e != hitEnemy && !e.IsDead())
            .OrderBy(e => Vector3.Distance(hitEnemy.transform.position, e.transform.position))
            .Take(2)
            .ToArray();

        // Fly to clicked enemy
        float flyDuration = CalculateFlightDuration(june.transform.position, hitEnemy.transform.position);
        yield return june.MoveJuneToPosition(hitEnemy.transform.position, flyDuration);

        // Chain to nearby enemies
        foreach (var target in targets)
        {
            if (target != null && !target.IsDead())
            {
                float duration = CalculateFlightDuration(june.transform.position, target.transform.position);
                yield return june.MoveJuneToPosition(target.transform.position, duration);
                target.OnEnemyClicked();
                Debug.Log($"Ricochet chain hit: {target.name}");
            }
        }

        yield return june.ReturnJuneHome();
    }

    private float CalculateFlightDuration(Vector3 from, Vector3 to)
    {
        return Vector3.Distance(from, to) / playerConfig.juneMoveSpeed;
    }
}
