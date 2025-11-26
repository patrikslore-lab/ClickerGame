using UnityEngine;
using System;
using Unity.VisualScripting;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Data")]
    [SerializeField] protected EnemyConfig enemyConfig; // ScriptableObject with stats
    
    // Events
    //public event Action<Enemy> OnDeath;
    protected float spawnTime;
    protected bool isDead = false;
    protected virtual void Start()
    {
        //RegisterWithManager();
        //PlaySpawnAnimation();
        spawnTime = Time.time;
    }
    
    protected virtual void RegisterWithManager()
    {

    }
    
    protected virtual void PlaySpawnAnimation()
    {

    }
    
    // Called by InputManager when clicked
    public virtual void OnEnemyClicked()
    {
        if (isDead) return;
        float timeTaken = EnemyDeathTimeTaken();
        // Notify listeners (CurrencyManager, UIManager, etc.)
        EventManager.Instance.TriggerClickTimeTaken(timeTaken);

        Die(timeTaken);
    }

    public virtual float EnemyDeathTimeTaken()
    {
        float timeTaken = (Time.time - spawnTime) * 1000f; // milliseconds
        return timeTaken;
    }
    
    protected virtual void Die(float reactionTime)
    {
        if (isDead) return;
        isDead = true;
        
        // Play death animation
        PlayDeathAnimation();
        
        // Notify EnemyManager
        //OnDeath?.Invoke(this);
    }

    protected virtual void PlayDeathAnimation()
    {
        // Override in subclasses for custom death animations        
        // Default: destroy after short delay
        Destroy(gameObject, 1f);
    }
    // Public getters
    public EnemyConfig GetEnemyData() => enemyConfig;
    public float GetSpawnTime() => spawnTime;
    public bool IsDead() => isDead;

    protected virtual void Destroy()
    {
        Destroy(gameObject);
    }
}