// LevelIntroController.cs
using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the intro sequence for a level.
/// Lives on the Level GameObject and is triggered by the state machine.
/// </summary>
public class LevelIntroController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool allowSkip = true;
    [SerializeField] private KeyCode skipKey = KeyCode.Space;
    
    private Coroutine activeIntro;
    private bool introComplete = false;

    /// <summary>
    /// Start the intro sequence
    /// </summary>
    public void PlayIntro()
    {
        if (activeIntro != null)
        {
            StopCoroutine(activeIntro);
        }
        
        introComplete = false;
        activeIntro = StartCoroutine(IntroSequence());
    }

    /// <summary>
    /// Skip the intro immediately
    /// </summary>
    public void SkipIntro()
    {
        if (activeIntro != null)
        {
            StopCoroutine(activeIntro);
            activeIntro = null;
        }
        
        introComplete = true;
        EventManager.Instance.TriggerLevelIntroComplete();
        Debug.Log("Intro skipped!");
    }

    private IEnumerator IntroSequence()
    {
        Debug.Log("=== Starting Level Intro ===");

        // Validate EventManager is available
        if (EventManager.Instance == null)
        {
            Debug.LogError("LevelIntroController: EventManager.Instance is NULL! Cannot run intro.");
            yield break;
        }

        // Phase 1: Lantern enters
        Debug.Log("Setting up Phase 1 handlers...");
        bool phase1Done = false;
        System.Action handler1 = () =>
        {
            Debug.Log("Phase 1 completion handler called!");
            phase1Done = true;
        };
        EventManager.Instance.OnIntroPhase1Complete += handler1;
        Debug.Log("Triggering Phase 1 event...");
        EventManager.Instance.TriggerIntroPhase1();
        Debug.Log("Waiting for Phase 1 to complete...");
        yield return new WaitUntil(() => phase1Done);
        EventManager.Instance.OnIntroPhase1Complete -= handler1;
        Debug.Log("Phase 1 complete");

        // Phase 2: Light activates
        bool phase2Done = false;
        System.Action handler2 = () => phase2Done = true;
        EventManager.Instance.OnIntroPhase2Complete += handler2;
        EventManager.Instance.TriggerIntroPhase2();
        yield return new WaitUntil(() => phase2Done);
        EventManager.Instance.OnIntroPhase2Complete -= handler2;
        Debug.Log("Phase 2 complete");

        // Phase 3
        //bool phase3Done = false;
        //System.Action handler3 = () => phase3Done = true;
        //EventManager.Instance.OnIntroPhase3Complete += handler3;
        //EventManager.Instance.TriggerIntroPhase3();
        //yield return new WaitUntil(() => phase3Done);
        //EventManager.Instance.OnIntroPhase3Complete -= handler3;
        //Debug.Log("Phase 3 complete");

        // Phase 4: Door animation - add in when complete ( uncomment )
        //bool phase4Done = false;
        //System.Action handler4 = () => phase4Done = true;
        //EventManager.Instance.OnIntroPhase4Complete += handler4;
        //EventManager.Instance.TriggerIntroPhase4();
        //yield return new WaitUntil(() => phase4Done);
        //EventManager.Instance.OnIntroPhase4Complete -= handler4;
        //Debug.Log("Phase 4 complete");

        Debug.Log("=== Level Intro Complete ===");
        activeIntro = null;
        introComplete = true;
        EventManager.Instance.TriggerLevelIntroComplete();
    }

    private void Update()
    {
        // Handle skip input
        if (allowSkip && activeIntro != null && Input.GetKeyDown(skipKey))
        {
            SkipIntro();
        }
    }

    public bool IsIntroComplete => introComplete;
}