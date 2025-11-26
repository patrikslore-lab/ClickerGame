using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Enemy/Enemy Config")]
public class EnemyConfig : ScriptableObject
{
    [Header("Prefab")]
    public GameObject enemyPrefab;

    [Header("Movement Settings")]
    public EnemyMovementType movementType = EnemyMovementType.Static;
    public float floatHeight = 2f;
    public float floatDuration = 5f;
    public Vector3 moveDirection = Vector3.zero;
    public float moveSpeed = 5f;

    [Header("Splitter Radius")]
    public float radius = 0.5f;
}

    


public enum EnemyMovementType
{
    Static,
    FloatUpDown,
    CentreResponsive,
    Custom
}