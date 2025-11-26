using UnityEngine;

[CreateAssetMenu(fileName = "LootConfig", menuName = "ScriptableObjects/LootConfig")]
public class LootConfig : ScriptableObject
{
    public enum ResourceType
    {
        Currency,
        Wood
    }

    [Header("Loot Properties")]
    public ResourceType resourceType;
    public int amount = 1;    
    [Header("Drop Settings")]
    [Range(0f, 1f)]
    public float dropChance = 0.2f; // 20% chance
    
    [Header("Collection")]
    public float despawnDuration = 0.3f; // Time to fade out after collection
}