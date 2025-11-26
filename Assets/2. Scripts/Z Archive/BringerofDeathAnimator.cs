using UnityEngine;
using System.Collections;

public class BringerOfDeathAnimator : MonoBehaviour
{
    // Reference to the Animator component
    private Animator animator;

    private float deathAnimationDuration = 1f;

    private System.Action onAnimationComplete;


    void Start()
    {
        // Get the Animator component attached to this GameObject
        animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogError("BringerOfDeathAnimator: No Animator component found!");
    }

    /// <summary>
    /// Plays the spawn animation (the Cast animation)
    /// </summary>
    public void PlaySpawnAnimation()
    {
        // Set the isSpawning parameter to true to trigger the Cast animation
        animator.SetBool("isSpawning", true);
    }

    /// <summary>
    /// Stops the spawn animation and transitions to idle
    /// </summary>
    public void StopSpawning()
    {
        // Set isSpawning to false so it transitions back to Idle
        animator.SetBool("isSpawning", false);
    }


    /// <summary>
    /// Returns the current animation state
    /// (Useful for checking if animation is still playing)
    /// </summary>
    public AnimatorStateInfo GetCurrentAnimationState()
    {
        return animator.GetCurrentAnimatorStateInfo(0);
    }


    public void PlayDieAnimation(System.Action callback = null)
    {
        onAnimationComplete = callback;
        animator.SetBool("isDying", true);
        StartCoroutine(WaitForAnimationComplete("Death", deathAnimationDuration));
    }

    private IEnumerator WaitForAnimationComplete(string stateName, float duration)
    {
        yield return new WaitForSeconds(duration);
        // Animation finished
        OnAnimationComplete();
    }

    private void OnAnimationComplete()
    {
        Debug.Log("Animation completed!");
        onAnimationComplete?.Invoke();  // Call the callback if provided
    }
}