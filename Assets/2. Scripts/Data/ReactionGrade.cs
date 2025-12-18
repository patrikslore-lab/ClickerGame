using UnityEngine;

/// <summary>
/// Data class representing a reaction time grade.
/// Contains the grade letter, color, and sprite for display.
/// </summary>
[System.Serializable]
public class ReactionGrade
{
    public enum Grade
    {
        S,  // < 300ms
        A,  // 300-500ms
        B,  // 500-800ms
        C,  // 800-1200ms
        D   // > 1200ms
    }
    // Grade thresholds (in milliseconds)
    public const float S_RANK_THRESHOLD = 500f;
    public const float A_RANK_THRESHOLD = 750f;
    public const float B_RANK_THRESHOLD = 1000f;
    public const float C_RANK_THRESHOLD = 1200f;

    /// <summary>
    /// Calculate grade from reaction time in milliseconds
    /// </summary>
    public static Grade CalculateGrade(float reactionTimeMs)
    {
        if (reactionTimeMs < S_RANK_THRESHOLD) return Grade.S;
        if (reactionTimeMs < A_RANK_THRESHOLD) return Grade.A;
        if (reactionTimeMs < B_RANK_THRESHOLD) return Grade.B;
        return  Grade.C;
    }
}
