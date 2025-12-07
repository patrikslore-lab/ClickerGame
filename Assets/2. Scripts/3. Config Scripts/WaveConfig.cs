using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Wave_1", menuName = "Game/Wave Config")]
public class WaveConfig : ScriptableObject
{
    [System.Serializable]
    public class EnemySpawnEntry
    {
        public EnemyConfig enemyConfig;
        [Min(1)] public int spawnCount = 1;
    }

    [SerializeField] private string waveName = "Wave";
    [SerializeField] private List<EnemySpawnEntry> enemiesToSpawn = new List<EnemySpawnEntry>();
    [Min(0)] [SerializeField] private float spawnVariation = 0.1f;

    public string WaveName => waveName;
    public List<EnemySpawnEntry> EnemiesToSpawn => enemiesToSpawn;
    public float SpawnVariation => spawnVariation;
}