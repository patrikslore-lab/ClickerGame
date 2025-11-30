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
    public float ricochetMaxActiveTime = 3f;

    [Header("Looter Ability")]
    public float looterMaxActiveTime = 5f;

    [Header("Loot Drop Rates")]
    public int woodDropAmount = 3;
    public int corePieceAmount = 1;

    [Header("Enemy Light Reduction Ratez")]
    public float flyingRatLightReductionRate = 0.2f;
    public float bringerOfDeathLightReductionRate = 0.5f;
    public float crowooonLightReductionRate = 1f;

    [Header("Enemy Light Reward Rates")]
    public float flyingRatLightRewardRate = 0.5f;
    public float bringerOfDeathLightRewardRate = 1f;
    public float crowooonLightRewardRate = 1f;

    [Header("June Stats")]
    public float juneCooldown = 20f;
    public Vector3 juneHomePosition = new Vector3(-5f, 3f, 0f);
    public float juneIdleMovementRadius = 0.3f;
    public float juneIdleMovementSpeed = 0.5f;
    public float juneMoveSpeed = 5f;
}