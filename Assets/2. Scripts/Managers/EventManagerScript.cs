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
    public event Action doorBreak1;
    public event Action doorBreak2;
    public event Action doorBreak3;
    
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
    public event Action ProtectorLightAddition;

    public void TriggerEnemyHit(Enemy enemy)
    {
        OnEnemyHit?.Invoke(enemy);
    }

    public void TriggerProtectorLightAddition()
    {
        ProtectorLightAddition?.Invoke();
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

    public void DoorBreak1()
    {
        doorBreak1?.Invoke();
    }   
    public void DoorBreak2()
    {
        doorBreak2?.Invoke();
    }   
    public void DoorBreak3()
    {
        doorBreak3?.Invoke();
    }   
}