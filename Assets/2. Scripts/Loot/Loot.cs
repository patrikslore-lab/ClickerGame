using UnityEngine;

public enum LootType { Wood, Core }

public class Loot : MonoBehaviour
{
    public LootType lootType;

    protected SpriteRenderer spriteRenderer;
    protected Collider2D lootCollider;
    protected Animator animator;
    protected PlayerConfig playerConfig;
    private bool isCollected = false;


    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        playerConfig = GameManager.Instance.GetPlayerConfig();

        EventManager.Instance.LootClicked += HandleLootClicked;

        gameObject.tag = "Loot";
    }

    /// Called by InputManager when loot is clicked
    private void HandleLootClicked(Loot loot)
    {
        if (loot == this)
        {
            if (isCollected) return;

            isCollected = true;
            LootManager.Instance.Collect(this);
        }
    }

    public void PlayDespawnAnimation()
    {
        // Disable collider first so it can't be clicked again
        if (lootCollider != null)
        {
            lootCollider.enabled = false;
        }
        
        // Disable renderer
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
        
        // Destroy after animation completes
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        EventManager.Instance.LootClicked -= HandleLootClicked;
    }
}