using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyListConfig", menuName = "Game/Enemy List Config")]
public class EnemyListConfig : ScriptableObject
{
    public List<GameObject> enemyPrefabs = new List<GameObject>();
}
