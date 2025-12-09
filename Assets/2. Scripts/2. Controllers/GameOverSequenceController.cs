using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using Unity.VisualScripting;
using System;

public class GameOverSequenceController : MonoBehaviour
{
    [SerializeField] private GameObject globalLight;

    private Light2D globalLight2DSettings;
    
    private Coroutine activeGameOverSequence;
    private bool gameOverComplete = false;

    public bool IsGameOverComplete => gameOverComplete;

    void Start()
    {
        globalLight2DSettings = globalLight.GetComponent<Light2D>();
    }
    public void PlayGameOverSequence()
    {
        if (activeGameOverSequence != null)
        {
            StopCoroutine(activeGameOverSequence);
        }
        
        gameOverComplete = false;
        activeGameOverSequence = StartCoroutine(GOSSequence());
    }


    private IEnumerator GOSSequence()
    {
        Debug.Log("=== Starting Game Over Sequence ===");

        PlayerManager.Instance.DisableJune();
        // Phase 1: Light dims to 0
        Debug.Log("Phase 1: Light Dimming");
        yield return DimGlobalLight(3f);
        Debug.Log("Phase 1 Darkness Complete");

        // Phase 2: Overwhelming enemy cores appear
        Debug.Log("Phase 2: Overwhelming...");
        yield return LevelManager.Instance.SpawnGameOverEnemies();
        Debug.Log("Phase 2 Overwhelm Complete");

        //CONVERGING COMMENTED OUT - UNCOMMENT TO ENABLE
        
        // Phase 3: Enemy Cores move to player position (above UI)
        //Debug.Log("Phase 3: UI revealing...");
        //yield return LevelManager.Instance.ConvergeOnPlayer();
        //Debug.Log("Phase 3 complete");


        activeGameOverSequence = null;
        CompleteIntro();
    }
        private void CompleteIntro()
    {
        gameOverComplete = true;
        UIManager.Instance.OnGameOverSequenceComplete();
        Debug.Log("=== Level Intro Complete ===");
    }

    private IEnumerator DimGlobalLight(float duration = 3f)
    {
        float startIntensity = globalLight2DSettings.intensity;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            globalLight2DSettings.intensity = Mathf.Lerp(startIntensity, 0f, t);
            yield return null;
        }

        globalLight2DSettings.intensity = 0f;
    }

}
