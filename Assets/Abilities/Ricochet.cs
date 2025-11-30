using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RicochetAbility : BaseAbility
{
    private enum RicochetState
    {
        Inactive,
        Active,
        OnCooldown
    }
    private RicochetState currentState = RicochetState.Inactive;
    private float activeTimer = 0f;

    public bool IsActive => currentState == RicochetState.Active;

    protected override void Start()
    {
        base.Start();
        EventManager.Instance.OnEnemyHit += ProcessRicochet;
    }

    private void OnDestroy()
    {
        EventManager.Instance.OnEnemyHit -= ProcessRicochet;
    }

    private void Update()
    {
        // Input handling
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!CanUseAbility())
            {
                Debug.Log("I'M ON COOLDOWN YA FANNY");
                return;
            }

            if (currentState == RicochetState.Inactive)
            {
                ActivateRicochet();
            }
            else if (currentState == RicochetState.Active)
            {
                DeactivateRicochet();
            }
        }

        // Handle auto-deactivation after max active time
        if (currentState == RicochetState.Active)
        {
            activeTimer += Time.deltaTime;
            if (activeTimer >= playerConfig.ricochetMaxActiveTime)
            {
                DeactivateRicochet();
            }
        }
    }

    private void ActivateRicochet()
    {
        currentState = RicochetState.Active;
        activeTimer = 0f;
        UIManager.Instance.RicochetActivate();
        Debug.Log("Ricochet ability: ACTIVE");
    }

    private void DeactivateRicochet()
    {
        currentState = RicochetState.Inactive;
        base.StartCooldown();
        UIManager.Instance.RicochetOnCooldown();
        Debug.Log("Ricochet deactivated - COOLDOWN started");
    }

    public void ProcessRicochet(Enemy hitEnemy)
    {
        if (currentState != RicochetState.Active) return;
        if (juneCharacter.IsPerformingAbility) return; // June is busy with another ricochet

        StartCoroutine(PerformRicochetSequence(hitEnemy));
    }

    private IEnumerator PerformRicochetSequence(Enemy hitEnemy)
    {
        // Take control of June
        juneCharacter.StartAbilityControl();

        // Find closest enemies
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy[] closestEnemies = FindClosestEnemies(hitEnemy, allEnemies, 2);

        // 1. Fly to clicked enemy
        float flyDuration = Vector3.Distance(juneCharacter.JuneInstance.transform.position, hitEnemy.transform.position) / playerConfig.juneMoveSpeed;
        yield return juneCharacter.MoveJuneToPosition(hitEnemy.transform.position, flyDuration);

        // 2. Fly to 1st closest enemy and hit it
        if (closestEnemies.Length > 0 && closestEnemies[0] != null && !closestEnemies[0].IsDead())
        {
            float duration1 = Vector3.Distance(juneCharacter.JuneInstance.transform.position, closestEnemies[0].transform.position) / playerConfig.juneMoveSpeed;
            yield return juneCharacter.MoveJuneToPosition(closestEnemies[0].transform.position, duration1);
            closestEnemies[0].OnEnemyClicked();
            Debug.Log($"June ricochet hit: {closestEnemies[0].name}");
        }

        // 3. Fly to 2nd closest enemy and hit it
        if (closestEnemies.Length > 1 && closestEnemies[1] != null && !closestEnemies[1].IsDead())
        {
            float duration2 = Vector3.Distance(juneCharacter.JuneInstance.transform.position, closestEnemies[1].transform.position) / playerConfig.juneMoveSpeed;
            yield return juneCharacter.MoveJuneToPosition(closestEnemies[1].transform.position, duration2);
            closestEnemies[1].OnEnemyClicked();
            Debug.Log($"June ricochet hit: {closestEnemies[1].name}");
        }

        // 4. Return home (also releases control back to JuneCharacter)
        yield return juneCharacter.ReturnJuneHome();
    }

    private Enemy[] FindClosestEnemies(Enemy fromEnemy, Enemy[] allEnemies, int count)
    {
        List<Enemy> validEnemies = new List<Enemy>();

        foreach (Enemy enemy in allEnemies)
        {
            if (enemy != fromEnemy && !enemy.IsDead())
            {
                validEnemies.Add(enemy);
            }
        }

        validEnemies.Sort((a, b) =>
            Vector3.Distance(fromEnemy.transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(fromEnemy.transform.position, b.transform.position))
        );

        int resultCount = Mathf.Min(count, validEnemies.Count);
        return validEnemies.GetRange(0, resultCount).ToArray();
    }
}
