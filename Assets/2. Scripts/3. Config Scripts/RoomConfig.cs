using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Room_X", menuName = "Game/Room Config")]
public class RoomConfig : ScriptableObject
{
    public enum DoorBreakTrigger
    {
        None,
        Break1,
        Break2,
        Break3
    }

    [System.Serializable]
    public class EnemySpawn
    {
        public GameObject enemyPrefab;
        [Min(1)] public int spawnCount = 1;
        [Tooltip("Delay between each enemy in this group spawning")]
        [Min(0)] public float delayBetweenEnemies = 0f;
        [Tooltip("Delay before next spawn entry")]
        [Min(0)] public float delayBeforeNext = 2f;
        [Tooltip("Trigger door break when all enemies in this spawn group are defeated")]
        public DoorBreakTrigger doorBreakOnDefeat = DoorBreakTrigger.None;

        [Header("Wave Dialogue (Optional)")]
        [Tooltip("Dialogue to play BEFORE this wave spawns")]
        public DialogueData dialogueBeforeWave;
        [Tooltip("Dialogue to play AFTER the first enemy is instantiated")]
        public DialogueData dialogueAfterSpawning;
        [Tooltip("Dialogue to play AFTER this wave is defeated")]
        public DialogueData dialogueAfterWave;
    }

    [SerializeField] private int roomNumber;
    [SerializeField] public Sprite roomSprite;

    [SerializeField] public GameObject levelGameObject;

    [SerializeField] public GameObject door;

    [Header("Enemy Spawns")]
    [SerializeField] private List<EnemySpawn> enemySpawns = new List<EnemySpawn>();

    [Header("Spawn Bounds")]
    [SerializeField] private float minX = -8f;
    [SerializeField] private float maxX = 8f;
    [SerializeField] private float minY = -4f;
    [SerializeField] private float maxY = 4f;
    [SerializeField] private float spawnZ = -1f;

    [Header("Loot")]
    [SerializeField] private int woodSpawnFrequencySeconds = 3;

    [Header("Loot Spawn Bounds")]
    [SerializeField] private float lootMinX = -6f;
    [SerializeField] private float lootMaxX = 6f;
    [SerializeField] private float lootMinY = -3f;
    [SerializeField] private float lootMaxY = 3f;
    [SerializeField] private float lootSpawnZ = -2f;

    public int WoodSpawnFrequencySeconds => woodSpawnFrequencySeconds;
    public float LootMinX => lootMinX;
    public float LootMaxX => lootMaxX;
    public float LootMinY => lootMinY;
    public float LootMaxY => lootMaxY;
    public float LootSpawnZ => lootSpawnZ;
    public int RoomNumber => roomNumber;
    public List<EnemySpawn> EnemySpawns => enemySpawns;
    public float MinX => minX;
    public float MaxX => maxX;
    public float MinY => minY;
    public float MaxY => maxY;
    public float SpawnZ => spawnZ;
    public Sprite RoomSprite => roomSprite;
}