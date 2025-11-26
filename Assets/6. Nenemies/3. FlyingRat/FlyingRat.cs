using UnityEngine;

public class FlyingRat : Enemy
{
    private Animator fRatAnimator;

    bool isCasting = false;

    protected override void Start()
    {
        fRatAnimator = GetComponent<Animator>();


        base.Start();
    }

    protected override void PlayDeathAnimation()
    {
         if (fRatAnimator != null)
        {
            fRatAnimator.SetBool("isDying", true);
        }
    }

    private void Update()
    {
        // Check if the CastPrep animation is playing
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

}
