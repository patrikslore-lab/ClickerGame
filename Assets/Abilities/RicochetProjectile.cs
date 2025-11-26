using UnityEngine;

public class RicochetProjectile : MonoBehaviour
{
    private Enemy targetEnemy;
    private Vector3 targetPosition;
    private float speed;
    private bool hasHit = false;

    public void Initialize(Enemy target, float moveSpeed)
    {
        targetEnemy = target;
        speed = moveSpeed;
        
        // Get the center of the target enemy's collider
        Collider2D targetCollider = target.GetComponent<Collider2D>();
        if (targetCollider != null)
        {
            targetPosition = targetCollider.bounds.center;
        }
        else
        {
            Debug.Log("RicochetProjectile: No collider found");
        }
    }

    private void Update()
    {
        if (targetEnemy == null || hasHit)
        {
            Destroy(gameObject);
            return;
        }

        // Move toward target position
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null && enemy == targetEnemy && !enemy.IsDead())
        {
            hasHit = true;
            Debug.Log($"Hit! {enemy.name}");
            enemy.OnEnemyClicked();
            Destroy(gameObject);
        }
    }
}