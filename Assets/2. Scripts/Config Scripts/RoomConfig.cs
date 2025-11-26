using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Room_X", menuName = "Game/Room Config")]
public class RoomConfig : ScriptableObject
{
    [System.Serializable]
    public class WaveReference
    {
        public WaveConfig waveConfig;
        [Min(1)] public int timesToRepeat = 1;
    }

    [SerializeField] private int roomNumber;

    [SerializeField] private Sprite roomSprite;
    [SerializeField] private List<WaveReference> waveSequence = new List<WaveReference>();
    [Min(0.5f)] [SerializeField] private float delayBetweenWaves = 3f;

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
    public List<WaveReference> WaveSequence => waveSequence;
    public float DelayBetweenWaves => delayBetweenWaves;
    public float MinX => minX;
    public float MaxX => maxX;
    public float MinY => minY;
    public float MaxY => maxY;
    public float SpawnZ => spawnZ;
    public Sprite RoomSprite => roomSprite;
}