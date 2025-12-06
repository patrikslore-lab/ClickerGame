using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Centralized registry for tracking all active enemies in the scene.
/// Replaces expensive FindObjectsByType calls with O(1) lookups.
/// Enemies self-register on Start() and unregister on OnDestroy().
/// </summary>
public class EnemyRegistry : MonoBehaviour
{
    public static EnemyRegistry Instance { get; private set; }

    private HashSet<Enemy> activeEnemies = new HashSet<Enemy>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterEnemy(Enemy enemy)
    {
        activeEnemies.Add(enemy);
        Debug.Log($"Enemy registered. Total active: {activeEnemies.Count}");
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
        Debug.Log($"Enemy unregistered. Total active: {activeEnemies.Count}");
    }

    public int ActiveEnemyCount => activeEnemies.Count;

    public IEnumerable<Enemy> GetAllEnemies() => activeEnemies;

    public void Clear()
    {
        activeEnemies.Clear();
        Debug.Log("EnemyRegistry cleared");
    }
}
