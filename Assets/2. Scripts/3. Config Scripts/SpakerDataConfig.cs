using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Speaker", menuName = "Game/Speaker Data")]
public class SpeakerData : ScriptableObject
{
    public enum Emotion
    {
        Neutral,
        Happy,
        Sad,
        Angry,
        Surprised,
        Worried
    }

    [System.Serializable]
    public class PortraitVariant
    {
        public Emotion emotion;
        public Sprite portrait;
    }

    [SerializeField] private string speakerName;
    [SerializeField] private Sprite defaultPortrait;
    [SerializeField] private List<PortraitVariant> portraitVariants = new List<PortraitVariant>();

    public string SpeakerName => speakerName;
    public Sprite DefaultPortrait => defaultPortrait;

    public Sprite GetPortrait(Emotion emotion)
    {
        foreach (var variant in portraitVariants)
        {
            if (variant.emotion == emotion)
                return variant.portrait;
        }
        return defaultPortrait;
    }
}