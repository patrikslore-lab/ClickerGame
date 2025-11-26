using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    
    public event Action<float> OnTargetClicked;
    public event Action<float> NewHighScore;
    public event Action <Enemy> LightDestruction;
    public event Action <Enemy> CoreHit;
    public event Action<Loot> LootClicked;
    public event Action CoreCollected;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes (optional)
    }
    
    public void TriggerClickTimeTaken(float timeTaken)
    {
        OnTargetClicked?.Invoke(timeTaken);
    }
    
    public void TriggerNewHighScore(float newHiScore)
    {
        NewHighScore?.Invoke(newHiScore);
    }
    
    public event Action<Enemy> OnEnemyHit;

    public void TriggerEnemyHit(Enemy enemy)
    {
        OnEnemyHit?.Invoke(enemy);
    }
    public void TriggerCoreHit(Enemy enemy)
    {
        CoreHit?.Invoke(enemy);
    }

    public void LightBeingDestroyed(Enemy enemy)
    {
        LightDestruction?.Invoke(enemy);
    }
    
    public void TriggerLootHit(Loot loot)
    {
        LootClicked?.Invoke(loot);
    }

    public void OnCoreCollection()
    {
        CoreCollected?.Invoke();
    }
}