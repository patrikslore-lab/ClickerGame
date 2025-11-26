
using UnityEngine;

public class HighScoreHandler : MonoBehaviour
{
    public GameObject highScoreBoxPrefab; // Reference to the prefab
    public float highScore = 2000; // Stores the lowest score
    public float latestTimeTaken;
    private float newScore;

    public delegate void OnSoundClicked(bool newHiScore);
    
    //public static event OnSoundClicked SoundClicked; // Event to notify when a target is clicked
    private void Start()
    {
        EventManager.Instance.OnTargetClicked += UpdateHighScore;
    }
    private void OnDestroy()
    {
        // Unsubscribe when destroyed to prevent memory leaks
        EventManager.Instance.OnTargetClicked -= UpdateHighScore;
    }
    public void UpdateHighScore(float timeTaken)
    {
        newScore = timeTaken;
        if (newScore < highScore)
        {
            highScore = newScore;
            PrintHighScore();
            EventManager.Instance.TriggerNewHighScore(timeTaken);
        }
    }
    public void PrintHighScore()
    {
        TextMesh textMesh = highScoreBoxPrefab.GetComponent<TextMesh>();
        textMesh.text = "Best Score:" + " " + highScore.ToString("F0") + " ms";
    }
}