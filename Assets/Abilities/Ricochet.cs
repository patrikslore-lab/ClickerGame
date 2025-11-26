using UnityEngine;
using System.Collections.Generic;

public class RicochetAbility : MonoBehaviour
{
    private enum RicochetState
    {
        Inactive,
        Active,
        OnCooldown
    }
    [SerializeField] private GameObject projectilePrefab;
    private PlayerConfig playerConfig; //player config file that holds all values (for future upgrading)
    private static RicochetAbility instance;
    private RicochetState currentState = RicochetState.Inactive;
    private float activeTimer = 0f;
    private float cooldownTimer = 0f;

    public bool IsActive => currentState == RicochetState.Active;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        EventManager.Instance.OnEnemyHit += ProcessRicochet;
        playerConfig = GameManager.Instance.GetPlayerConfig();
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
            if (currentState == RicochetState.Inactive)
            {
                ActivateRicochet();
            }
            else if (currentState == RicochetState.Active)
            {
                StartCooldown();
            }
            else if (currentState == RicochetState.OnCooldown)
            {
                Debug.Log("I'M ON COOLDOWN YA FANNY");
            }
        }

        // Handle auto-deactivation after 3 seconds
        if (currentState == RicochetState.Active)
        {
            activeTimer += Time.deltaTime;
            if (activeTimer >= playerConfig.ricochetMaxActiveTime)
            {
                StartCooldown();
            }
        }

        //  Handle cooldown countdown
        if (currentState == RicochetState.OnCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= playerConfig.ricochetCooldown)
            {
                currentState = RicochetState.Inactive;
                UIManager.Instance.RicochetAvailable();
                Debug.Log("Ricochet cooldown complete - AVAILABLE");
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

    private void StartCooldown()
    {
        currentState = RicochetState.OnCooldown;
        cooldownTimer = 0f;
        UIManager.Instance.RicochetOnCooldown();
        Debug.Log("Ricochet deactivated - COOLDOWN started");
    }

    public void ProcessRicochet(Enemy hitEnemy)
    {
        if (currentState != RicochetState.Active) return;

        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        Enemy[] closestEnemies = FindClosestEnemies(hitEnemy, allEnemies, 2);

        if (closestEnemies.Length == 0)
        {
            Debug.LogWarning("Ricochet: No other enemies to bounce to");
            return;
        }

        foreach (Enemy enemy in closestEnemies)
        {
            if (enemy != null && !enemy.IsDead())
            {
                SpawnProjectile(hitEnemy, enemy);
                Debug.Log($"Ricochet projectile spawned toward: {enemy.name}");
            }
        }
    }

    private void SpawnProjectile(Enemy fromEnemy, Enemy targetEnemy)
    {
        Collider2D fromCollider = fromEnemy.GetComponent<Collider2D>();
        Vector3 spawnPosition = fromEnemy.transform.position;

        if (fromCollider != null)
        {
            spawnPosition = fromCollider.bounds.center;
        }

        GameObject projectileObj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        RicochetProjectile projectile = projectileObj.GetComponent<RicochetProjectile>();

        if (projectile != null)
        {
            projectile.Initialize(targetEnemy, playerConfig.ricochetProjectileSpeed);
        }
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