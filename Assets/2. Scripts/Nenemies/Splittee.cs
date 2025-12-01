using UnityEngine;
using System.Collections;

public class Splittee : Enemy
{
    [Header("Jump Movement")]
    [SerializeField] private float jumpDuration = 0.5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float pauseBetweenJumps = 0.2f;
    [SerializeField] private float jumpDistance = 2f;
    private SpriteRenderer spriteRenderer;
    private RoomConfig config;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        config = LevelManager.Instance.GetCurrentRoomConfig();   
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Start jumping after spawn
        StartCoroutine(JumpingRoutine());
    }
    //override below to not include any delay after dying = can remove later 
    // (if we're having 1s death anims as standard)
    protected override void PlayDeathAnimation()
    {
        Destroy(gameObject);
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
        RoomConfig config = LevelManager.Instance.GetCurrentRoomConfig();

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

        // Log before clamping
        Debug.Log($"Splitter Jump: Current={currentPos}, Direction={randomDirection}, Offset={offset}, Target(unclamped)={targetPos}");

        // Clamp to room bounds
        targetPos.x = Mathf.Clamp(targetPos.x, config.MinX, config.MaxX);
        targetPos.y = Mathf.Clamp(targetPos.y, config.MinY, config.MaxY);
        targetPos.z = currentPos.z;

        // Log after clamping
        Debug.Log($"Splitter Jump: Target(clamped)={targetPos}, Bounds=[{config.MinX},{config.MaxX}] x [{config.MinY},{config.MaxY}], Distance={Vector3.Distance(currentPos, targetPos):F2}");

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
}


