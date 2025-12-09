using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Game/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public enum Position
    {
        Left,
        Right
    }

    [System.Serializable]
    public class DialogueLine
    {
        public Position position;
        public SpeakerData speaker;
        public SpeakerData.Emotion emotion = SpeakerData.Emotion.Neutral;
        [TextArea(2, 4)] public string text;
        [Tooltip("0 = wait for click, >0 = auto-advance after X seconds")]
        public float autoAdvanceTime = 0f;
    }

    [SerializeField] private List<DialogueLine> lines = new List<DialogueLine>();

    public List<DialogueLine> Lines => lines;
}