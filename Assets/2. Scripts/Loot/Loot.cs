using UnityEngine;

public enum LootType { Wood, Core }

/// <summary>
/// Base class for loot items.
/// Collection handled via direct calls from InputManager -> LevelManager -> LootController
/// </summary>
public class Loot : MonoBehaviour
{
    public LootType lootType;

    protected SpriteRenderer spriteRenderer;
    protected Collider2D lootCollider;
    protected Animator animator;
    private bool isCollected = false;

    public bool IsCollected => isCollected;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lootCollider = GetComponent<Collider2D>();
        gameObject.tag = "Loot";
    }

    /// <summary>
    /// Called by InputManager when this loot is clicked.
    /// Routes through LevelManager -> LootController for collection.
    /// </summary>
    public void OnLootClicked()
    {
        if (isCollected) return;

        isCollected = true;
        LevelManager.Instance?.CollectLoot(this);
    }

    /// <summary>
    /// Play despawn animation and destroy the loot object.
    /// Called by LootController after collection.
    /// </summary>
    public void PlayDespawnAnimation()
    {
        if (lootCollider != null)
            lootCollider.enabled = false;

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        Destroy(gameObject);
    }
}
