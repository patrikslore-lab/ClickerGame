using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "ScriptableObjects/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [Header("Resources")]
    public float corePieces = 0f;
    public float wood = 0f;

    [Header("Level")]
    public int currentLevel = 1;

    [Header("Ricochet Ability")]
    public float ricochetCooldown = 5f;
    public float ricochetProjectileSpeed = 10f;
    public float ricochetMaxActiveTime = 3f;

    [Header("Loot Drop Rates")]
    public int woodDropAmount = 3;
    public int corePieceAmount = 1;

    [Header("Enemy Light Reduction Rates")]
    public float flyingRatLightReductionRate = 0.2f;
    public float bringerOfDeathLightReductionRate = 0.5f;

    [Header("Enemy Light Reward Rates")]
    public float flyingRatLightRewardRate = 0.5f;
    public float bringerOfDeathLightRewardRate = 1f;
}