using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuAnimation : MonoBehaviour
{
    private Image backgroundImage;
    [SerializeField] private Sprite sprite1;
    [SerializeField] private Sprite sprite2;
    [SerializeField] private float frameTime = 0.5f;
    private Coroutine animationCoroutine;

    void OnEnable()
    {
        // Start animation when the GameObject becomes active
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        backgroundImage = GetComponent<Image>();

        if (backgroundImage == null)
        {
            Debug.LogError("MainMenuAnimation: No Image component found on this GameObject!");
            return;
        }

        if (sprite1 == null || sprite2 == null)
        {
            Debug.LogError("MainMenuAnimation: sprite1 or sprite2 not assigned in Inspector!");
            return;
        }

        animationCoroutine = StartCoroutine(AnimateBackground());
    }

    void OnDisable()
    {
        // Stop animation when the GameObject becomes inactive
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
    }

    IEnumerator AnimateBackground()
    {
        while(true)
        {
            if (backgroundImage != null && sprite1 != null)
            {
                backgroundImage.sprite = sprite1;
            }

            yield return new WaitForSeconds(frameTime);

            if (backgroundImage != null && sprite2 != null)
            {
                backgroundImage.sprite = sprite2;
            }

            yield return new WaitForSeconds(frameTime);
        }
    }
}
