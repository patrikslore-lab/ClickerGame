// LevelIntroController.cs
using UnityEngine;
using System.Collections;

public class LevelIntroController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool allowSkip = true;
    [SerializeField] private KeyCode skipKey = KeyCode.Space;
    
    [Header("Phase Controllers")]
    [SerializeField] private LanternController lanternController;
    
    private Coroutine activeIntro;
    private bool introComplete = false;

    public bool IsIntroComplete => introComplete;

    public void PlayIntro()
    {
        if (activeIntro != null)
        {
            StopCoroutine(activeIntro);
        }
        
        introComplete = false;
        activeIntro = StartCoroutine(IntroSequence());
    }

    public void SkipIntro()
    {
        if (activeIntro == null) return;
        
        StopCoroutine(activeIntro);
        activeIntro = null;
        
        // Snap everything to final state
        lanternController?.SnapToFinalState();
        UIManager.Instance?.ShowGameplayUI();
        
        CompleteIntro();
    }

    private void CompleteIntro()
    {
        introComplete = true;
        EventManager.Instance?.TriggerLevelIntroComplete();
        Debug.Log("=== Level Intro Complete ===");
    }

    private IEnumerator IntroSequence()
    {
        Debug.Log("=== Starting Level Intro ===");

        // Phase 1: Lantern moves to center
        Debug.Log("Phase 1: Lantern entering...");
        yield return lanternController.MoveToCenter();
        Debug.Log("Phase 1 complete");

        // Phase 2: Light activates
        Debug.Log("Phase 2: Light activating...");
        yield return lanternController.ActivateLight();
        Debug.Log("Phase 2 complete");

        // Phase 3: UI reveals
        Debug.Log("Phase 3: UI revealing...");
        yield return UIManager.Instance.RevealGameplayUI(1f);
        Debug.Log("Phase 3 complete");

        // Phase 4: Door animation (uncomment when ready)
        // Debug.Log("Phase 4: Door animating...");
        // yield return doorController.PlayEntryAnimation();
        // Debug.Log("Phase 4 complete");

        activeIntro = null;
        CompleteIntro();
    }

    private void Update()
    {
        if (allowSkip && activeIntro != null && Input.GetKeyDown(skipKey))
        {
            SkipIntro();
        }
    }
}