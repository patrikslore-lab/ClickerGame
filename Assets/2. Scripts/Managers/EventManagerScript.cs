using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
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
    public event Action<float> OnTargetClicked;
    public void TriggerClickTimeTaken(float timeTaken) => OnTargetClicked?.Invoke(timeTaken);

    public event Action<float> NewHighScore;
    public void TriggerNewHighScore(float newHiScore) => NewHighScore?.Invoke(newHiScore);

    public event Action<Enemy> OnEnemyHit;
    public void TriggerEnemyHit(Enemy enemy) => OnEnemyHit?.Invoke(enemy);

    public event Action ProtectorLightAddition;
    public void TriggerProtectorLightAddition() => ProtectorLightAddition?.Invoke();

    public event Action <Enemy> CoreHit;
    public void TriggerCoreHit(Enemy enemy) => CoreHit?.Invoke(enemy);

    public event Action <Enemy> LightDestruction;
    public void LightBeingDestroyed(Enemy enemy) => LightDestruction?.Invoke(enemy);

    public event Action<Loot> LootClicked;
    public void TriggerLootHit(Loot loot) => LootClicked?.Invoke(loot);

    public event Action CoreCollected;
    public void OnCoreCollection() => CoreCollected?.Invoke();

    //Level Exit Shadow Door Events
    public event Action doorBreak1;
    public event Action doorBreak2;
    public event Action doorBreak3;
    public void DoorBreak1() => doorBreak1?.Invoke();
    public void DoorBreak2() => doorBreak2?.Invoke();
    public void DoorBreak3() => doorBreak3?.Invoke();

    //Level Intro GameState Events======================================
    public event Action OnAllEnemiesDefeated;
    public event Action OnLightDepleted;

    public void TriggerAllEnemiesDefeated() => OnAllEnemiesDefeated?.Invoke();
    public void TriggerLightDepleted() => OnLightDepleted?.Invoke();

    public event Action OnLevelIntroComplete;
    public void TriggerLevelIntroComplete() => OnLevelIntroComplete?.Invoke();

    //Level Intro GameState Events End ======================================


}