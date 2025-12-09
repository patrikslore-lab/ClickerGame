// IAbility.cs

using JetBrains.Annotations;

/// <summary>
/// Simple interface for abilities. No inheritance hierarchy needed.
/// </summary>
public interface IAbility
{
    void Activate();
    void Deactivate();
    bool IsUnlocked { get; }
}
