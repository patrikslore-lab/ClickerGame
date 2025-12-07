using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Enemy/Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    [Header("Prefab")]
    public GameObject enemyPrefab;

    [Header("Splitter Radius")]
    public float radius = 0.5f;
}