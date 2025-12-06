using UnityEngine;
using System.Collections;

public class Splitter : Enemy
{
    private Animator splitterAnimator;
    [SerializeField] private GameObject splitteeOne;
    [SerializeField] private GameObject splitteeTwo;
    [SerializeField] private GameObject splitteeThree;
    private float splitteeSpawnRadius;

    [Header("Jump Movement")]
    [SerializeField] private float jumpDuration = 0.5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float pauseBetweenJumps = 0.2f;
    [SerializeField] private float jumpDistance = 2f;

    private RoomConfig config;
    private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();

        splitterAnimator = GetComponent<Animator>();
        if (splitterAnimator == null)
        {
            Debug.LogWarning("Splitter: Animator not found!");
        }

        splitteeSpawnRadius = enemyConfig.radius;
        config = LevelManager.Instance?.CurrentRoomConfig;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Start jumping after spawn
        StartCoroutine(JumpingRoutine());
    }

    protected override void PlaySpawnAnimation()
    {
        // Empty - no spawn animation for Splitter
    }

    protected override void PlayDeathAnimation()
    {
        if (splitterAnimator != null)
        {
            splitterAnimator.SetBool("isDying", true);
        }

        Destroy(gameObject);
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
        Vector2 center = new Vector2(transform.position.x, transform.position.y);
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

    private IEnumerator JumpingRoutine()
    {
        while (!isDead)
        {
            // Pause before next jump
            yield return new WaitForSeconds(pauseBetweenJumps);

            // Calculate next jump position
            Vector3 targetPosition = CalculateNextJumpPosition();

            // Perform the jump
            yield return StartCoroutine(JumpToPosition(targetPosition));
        }
    }

    private Vector3 CalculateNextJumpPosition()
    {
        if (config == null)
        {
            Debug.LogWarning("Splitter: No RoomConfig found! Using current position.");
            return transform.position;
        }

        Vector3 currentPos = transform.position;

        // Pick random direction (normalized vector)
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        // Apply fixed jump distance in that direction
        Vector3 offset = (Vector3)randomDirection * jumpDistance;
        Vector3 targetPos = currentPos + offset;

        // Clamp to room bounds
        targetPos.x = Mathf.Clamp(targetPos.x, config.MinX, config.MaxX);
        targetPos.y = Mathf.Clamp(targetPos.y, config.MinY, config.MaxY);
        targetPos.z = currentPos.z;

        return targetPos;
    }

    private IEnumerator JumpToPosition(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        // Flip sprite based on direction
        float direction = targetPosition.x - startPosition.x;
        if (spriteRenderer != null)
        {
            // Flip sprite if moving left (base prefab faces right)
            spriteRenderer.flipX = direction < 0;
        }

        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / jumpDuration;

            // Horizontal movement (lerp)
            float xPos = Mathf.Lerp(startPosition.x, targetPosition.x, t);
            float yPos = Mathf.Lerp(startPosition.y, targetPosition.y, t);

            // Vertical sine curve for jump arc
            float arcHeight = Mathf.Sin(t * Mathf.PI) * jumpHeight;

            transform.position = new Vector3(xPos, yPos + arcHeight, startPosition.z);

            yield return null;
        }

        // Ensure final position is exact
        transform.position = targetPosition;
    }

    private void OnDestroy()
    {
        // Stop jumping coroutine when destroyed
        StopAllCoroutines();
    }
}