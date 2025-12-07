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
    }

    //===========================================
    // CLICK/REACTION EVENTS
    //===========================================

    public event Action<float> OnTargetClicked;
    public void TriggerClickTimeTaken(float timeTaken) => OnTargetClicked?.Invoke(timeTaken);

    public event Action<float> NewHighScore;
    public void TriggerNewHighScore(float newHiScore) => NewHighScore?.Invoke(newHiScore);

    //===========================================
    // ENEMY EVENTS
    //===========================================

    public event Action<Enemy> OnEnemyHit;
    public void TriggerEnemyHit(Enemy enemy) => OnEnemyHit?.Invoke(enemy);

    //===========================================
    // LIGHT/LANTERN EVENTS (broadcast patterns)
    //===========================================

    /// <summary>
    /// Broadcast: Enemy is attacking the light (called every frame during cast).
    /// Listened by: LanternController
    /// </summary>
    public event Action<Enemy> LightDestruction;
    public void LightBeingDestroyed(Enemy enemy) => LightDestruction?.Invoke(enemy);

    /// <summary>
    /// Broadcast: Protector ability is active (called every frame).
    /// Listened by: LanternController
    /// </summary>
    public event Action ProtectorLightAddition;
    public void TriggerProtectorLightAddition() => ProtectorLightAddition?.Invoke();

    //===========================================
    // LEVEL EVENTS
    //===========================================

    public event Action OnAllEnemiesDefeated;
    public void TriggerAllEnemiesDefeated() => OnAllEnemiesDefeated?.Invoke();

    public event Action OnLightDepleted;
    public void TriggerLightDepleted() => OnLightDepleted?.Invoke();

    public event Action OnLevelIntroComplete;
    public void TriggerLevelIntroComplete() => OnLevelIntroComplete?.Invoke();

    //===========================================
    // DOOR EVENTS
    //===========================================

    public event Action doorBreak1;
    public event Action doorBreak2;
    public event Action doorBreak3;
    public void DoorBreak1() => doorBreak1?.Invoke();
    public void DoorBreak2() => doorBreak2?.Invoke();
    public void DoorBreak3() => doorBreak3?.Invoke();
}
