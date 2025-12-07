using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    private AudioSource soundPlayer;
    public AudioClip clickSound;
    public AudioClip hiScoreSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        soundPlayer = GetComponent<AudioSource>();

    }
    private void OnEnable()
    {
        EventManager.Instance.OnTargetClicked += ClickSound;
        EventManager.Instance.NewHighScore += HiScoreSound;
    }
    // Update is called once per frame
    private void OnDisable()
    {
        EventManager.Instance.OnTargetClicked -= ClickSound;
        EventManager.Instance.NewHighScore -= HiScoreSound;
    }

    void ClickSound(float timeTaken)
    {
        if (timeTaken > 0)
        {
            soundPlayer.PlayOneShot(clickSound, 1.0f);
        }
    }
    void HiScoreSound(float newHiScore)
    {
        if (newHiScore > 0)
        {
            soundPlayer.PlayOneShot(hiScoreSound, 1.0f);
        }
    }
}

