using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Splitter : Enemy
{
    private Animator splitterAnimator; //calls animator component directly
    [SerializeField] private GameObject splitteeOne;
    [SerializeField] private GameObject splitteeTwo;
    [SerializeField] private GameObject splitteeThree;
    private float splitteeSpawnRadius;
    protected override void Start()
    {
        splitterAnimator = GetComponent<Animator>();
        
        if (splitterAnimator == null)
        {
            Debug.LogWarning("BringerOfDeathAnimator not found!");
        }
        
        base.Start();

        splitteeSpawnRadius = enemyConfig.radius;
    }
    
    protected override void PlaySpawnAnimation()
    {

    }
    
    protected override void PlayDeathAnimation()
    {
        if (splitterAnimator != null)
        {
            splitterAnimator.SetBool("isDying", true);
        }
        
        // Destroy after animation plays (1 second)
        Destroy(gameObject); // , seconds to delay in line with anim
    }

    // Example: Add boss-specific behavior
    public void TriggerSpecialAttack()
    {
        Debug.Log("Splitter Special Attack!");
        // Custom boss logic here
    }

    public override void OnEnemyClicked()
    {
        if (isDead) return;
        
        float timeTaken = (Time.time - spawnTime) * 1000f; // milliseconds

        // Notify listeners (CurrencyManager, UIManager, etc.)
        EventManager.Instance.TriggerClickTimeTaken(timeTaken);

        Die(timeTaken);
        SpawnSplittees();
    }

    private void SpawnSplittees()
    {
        int i = 0;
        Instantiate(splitteeOne, SpawnCalc(i), Quaternion.identity);
        Instantiate(splitteeTwo, SpawnCalc(i+1), Quaternion.identity);
        Instantiate(splitteeThree, SpawnCalc(i+2), Quaternion.identity);
    }

    
    public Vector2 SpawnCalc(int i)
    {
        Vector2 center = new Vector2 (transform.position.x, transform.position.y);
        Vector2[] circlePositions = GetCirclePositions(center, splitteeSpawnRadius);
        Vector2 spawnPos = circlePositions[i];
        return spawnPos;
    }
        public Vector2[] GetCirclePositions(Vector2 center, float radius, int count = 3)
    {
        Vector2[] positions = new Vector2[count];
        
        for (int i = 0; i < count; i++)
        {
            float angle = i * (360f / count) * Mathf.Deg2Rad;
            positions[i] = center + new Vector2(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius
            );
        }
        
        return positions;
    }
}
