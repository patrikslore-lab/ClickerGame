using UnityEngine;
using System.Collections;

public class FlyingRat : Enemy
{
    [Header("Animation")]
    private Animator fRatAnimator;
    private bool isCasting = false;

    [Header("Movement Settings")]
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 3f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float pauseDuration = 0.5f;
    [SerializeField] private float arcHeight = 0.3f;

    private Coroutine movementCoroutine;
    private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        fRatAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        base.Start();
    }

    protected override void PlayDeathAnimation()
    {
        if (fRatAnimator != null)
        {
            fRatAnimator.SetBool("isDying", true);
        }
    }

    protected override void Die(float reactionTime)
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }

        base.Die(reactionTime);
    }

    private void Update()
    {
        // Check if the CastLoop animation is playing
        if (fRatAnimator != null && !isCasting && !isDead)
        {
            AnimatorStateInfo stateInfo = fRatAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("CastLoop"))
            {
                isCasting = true;
                Debug.Log("CastLoop animation detected - starting light reduction");
            }
        }

        // While casting, continuously reduce light
        if (fRatAnimator != null && isCasting && !isDead)
        {
            EventManager.Instance.LightBeingDestroyed(this);
        }
    }

    // ============ MOVEMENT SYSTEM ============

    // Called by animation event
    public void StartMovement()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }

        movementCoroutine = StartCoroutine(MovementLoop());
        Debug.Log("FlyingRat: StartMovement called - beginning flight");
    }

    private IEnumerator MovementLoop()
    {
        while (!isDead)
        {
            // Calculate random target position
            Vector3 targetPosition = GetRandomTargetPosition(out bool hitWall);

            // Move to target
            yield return StartCoroutine(MoveToPosition(targetPosition));

            // Only pause if we didn't hit a wall
            if (!hitWall)
            {
                yield return new WaitForSeconds(pauseDuration);
            }
            // If hit wall, immediately pick new target and continue
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float distance = Vector3.Distance(startPosition, targetPosition);
        float duration = distance / moveSpeed;

        // Calculate movement direction and perpendicular for arc
        Vector3 moveDirection = (targetPosition - startPosition).normalized;
        Vector3 perpendicular = new Vector3(-moveDirection.y, moveDirection.x, 0f);

        // Flip sprite based on movement direction (right-facing sprite is default)
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = moveDirection.x < 0; // Flip when moving left
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Linear interpolation along movement path - constant speed
            Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, t);

            // Add arc perpendicular to movement direction
            float arc = Mathf.Sin(t * Mathf.PI) * arcHeight;
            currentPos += perpendicular * arc;

            transform.position = currentPos;

            yield return null;
        }

        transform.position = targetPosition;
    }


    private Vector3 GetRandomTargetPosition(out bool hitWall)
    {
        RoomConfig config = GetRoomConfig();
        hitWall = false;

        if (config == null)
        {
            return transform.position;
        }

        // Generate random direction and guaranteed distance from current position
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float randomDistance = Random.Range(minDistance, maxDistance);

        Vector3 direction = new Vector3(
            Mathf.Cos(randomAngle),
            Mathf.Sin(randomAngle),
            0f
        ).normalized;

        // Calculate target position with guaranteed distance
        Vector3 targetPosition = transform.position + (direction * randomDistance);

        // Store original target to detect if clamping occurred
        Vector3 originalTarget = targetPosition;

        // Clamp to bounds with arc buffer
        float arcBuffer = arcHeight;
        targetPosition.x = Mathf.Clamp(targetPosition.x, config.MinX + arcBuffer, config.MaxX - arcBuffer);
        targetPosition.y = Mathf.Clamp(targetPosition.y, config.MinY + arcBuffer, config.MaxY - arcBuffer);
        targetPosition.z = config.SpawnZ;

        // Check if we hit a wall (clamping changed the position)
        if (Mathf.Abs(originalTarget.x - targetPosition.x) > 0.01f ||
            Mathf.Abs(originalTarget.y - targetPosition.y) > 0.01f)
        {
            hitWall = true;
        }

        return targetPosition;
    }

    private RoomConfig GetRoomConfig()
    {
        return LevelManager.Instance?.CurrentRoomConfig;
    }
}