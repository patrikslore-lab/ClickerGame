using UnityEngine;
using System.Collections;

public class SpawnManagerScript : MonoBehaviour
{
    public static SpawnManagerScript Instance { get; private set; }

    [SerializeField] private GameObject woodLootPrefab;

    [SerializeField] private GameObject coreLootPrefab;

    [SerializeField] private GameObject floatingTextPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    //Enemy spawning logic-----------------------------------------------------------
    public void SpawnEnemy(EnemyConfig config, Vector3 position, RoomConfig roomConfig)
    {
        GameObject enemy = Instantiate(config.enemyPrefab, position, Quaternion.identity);

        // Apply movement behavior
        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            enemyMovement.ApplyMovement(config, roomConfig);
        }
    } 
    public Vector3 CalculateSpawnPosition(RoomConfig roomConfig)
    {
        float randomX = Random.Range(roomConfig.MinX, roomConfig.MaxX);
        float randomY = Random.Range(roomConfig.MinY, roomConfig.MaxY);
        return new Vector3 (randomX, randomY, roomConfig.SpawnZ);
    }

    // Loot spawning logic ----------------------------------------------------------
    public void WoodSpawningLogic(RoomConfig roomConfig)
    {
        if (woodLootPrefab == null)
        {
            Debug.LogError("SpawnManagerScript: Wood loot prefab not assigned!");
            return;
        }

        Vector3 randomPos = CalculateRootPosition(roomConfig);

        float woodLootFrequency = roomConfig.WoodSpawnFrequencySeconds;

        StartCoroutine(SpawnWoodLoot(randomPos, woodLootFrequency, roomConfig));
    }

    private IEnumerator SpawnWoodLoot(Vector3 randomPos, float woodLootFrequency, RoomConfig roomConfig)
    {
        while (true)
        {
            yield return new WaitForSeconds(woodLootFrequency);
            Instantiate(woodLootPrefab, randomPos, Quaternion.identity);
            randomPos = CalculateRootPosition(roomConfig);
        }
    }
        public Vector3 CalculateRootPosition(RoomConfig roomConfig)
    {
        float randomX = Random.Range(roomConfig.LootMinX, roomConfig.LootMaxX);
        float randomY = Random.Range(roomConfig.LootMinY, roomConfig.LootMaxY);
        return new Vector3(randomX, randomY, roomConfig.LootSpawnZ);        
    }

    // CORE loot item spawning mechanic ---------------------------------------------

    public void SpawnCoreLoot(Enemy enemy)
    {
        if (coreLootPrefab == null)
        {
            Debug.LogError("SpawnManagerScript: Core loot prefab not assigned!");
            return;
        }
        if (enemy == null)
        {
            return;
        }
        GameObject coreLoot = Instantiate(coreLootPrefab, enemy.transform.position, Quaternion.identity);
        CoreLootFlight flightController = coreLoot.GetComponent<CoreLootFlight>();
        flightController.FlyToUI();

        //perfect textbox spawner logic
        PerfectTextBoxSpawner(enemy.transform.position);
    }

    //accompanying PERFECT textbox

    public void PerfectTextBoxSpawner(Vector3 position)
    {
        if (floatingTextPrefab == null)
        {
            Debug.LogError("SpawnManagerScript: Floating text prefab not assigned!");
            return;
        }

        // Offset the text above the enemy
        Vector3 textPosition = position + Vector3.up * 4f;
        GameObject floatingText = Instantiate(floatingTextPrefab, textPosition, Quaternion.identity);

        FloatingText floatingTextComponent = floatingText.GetComponent<FloatingText>();

        if (floatingTextComponent != null)
        {
            floatingTextComponent.Initialize();
        }
    }
}

