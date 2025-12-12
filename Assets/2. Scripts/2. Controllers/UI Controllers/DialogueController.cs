using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Handles dialogue display with typewriter effect.
/// Supports two speakers (left/right panels) and auto-advance for callouts.
/// Orchestrated by UIManager.
/// </summary>
public class DialogueUIController : MonoBehaviour
{
    [Header("Left Speaker Panel")]
    [SerializeField] private GameObject leftPanel;
    [SerializeField] private Image leftPortrait;
    [SerializeField] private TextMeshProUGUI leftText;

    [Header("Right Speaker Panel")]
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private Image rightPortrait;
    [SerializeField] private TextMeshProUGUI rightText;

    [Header("Typewriter Settings")]
    [SerializeField] private float charactersPerSecond = 30f;

    // State
    private DialogueData currentDialogue;
    private int currentLineIndex;
    private bool isPlaying;
    private bool isTyping;
    private Coroutine typewriterCoroutine;
    private Coroutine autoAdvanceCoroutine;

    // Current line references
    private TextMeshProUGUI activeText;
    private string fullText;

    public bool IsPlaying => isPlaying;

    private void Update()
    {
        if (!isPlaying) return;

        // Click to advance (only for non-auto-advance lines)
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                CompleteTypewriter();
            }
            else if (currentDialogue.Lines[currentLineIndex].autoAdvanceTime <= 0f)
            {
                AdvanceLine();
            }
        }
    }

    //===========================================
    // PUBLIC API
    //===========================================

    public void StartDialogue(DialogueData data)
    {
        if (data == null || data.Lines.Count == 0)
        {
            Debug.LogWarning("DialogueUIController: No dialogue data or empty lines");
            return;
        }

        currentDialogue = data;
        currentLineIndex = 0;
        isPlaying = true;

        HideBothPanels();
        DisplayCurrentLine();

        Debug.Log($"Dialogue started: {data.name}");
    }

    public void AdvanceLine()
    {
        if (!isPlaying) return;

        // Stop any auto-advance in progress
        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        // Hide current panel
        HideBothPanels();

        currentLineIndex++;

        if (currentLineIndex >= currentDialogue.Lines.Count)
        {
            EndDialogue();
        }
        else
        {
            DisplayCurrentLine();
        }
    }

    public void EndDialogue()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        HideBothPanels();
        isPlaying = false;
        isTyping = false;
        currentDialogue = null;

        Debug.Log("Dialogue ended");
    }

    //===========================================
    // DISPLAY
    //===========================================

    private void DisplayCurrentLine()
    {
        DialogueData.DialogueLine line = currentDialogue.Lines[currentLineIndex];

        // Get portrait from speaker data
        Sprite portrait = line.speaker != null 
            ? line.speaker.GetPortrait(line.emotion) 
            : null;

        // Show correct panel
        if (line.position == DialogueData.Position.Left)
        {
            leftPanel.SetActive(true);
            if (portrait != null) leftPortrait.sprite = portrait;
            activeText = leftText;
        }
        else
        {
            rightPanel.SetActive(true);
            if (portrait != null) rightPortrait.sprite = portrait;
            activeText = rightText;
        }

        // Start typewriter
        fullText = line.text;
        typewriterCoroutine = StartCoroutine(TypewriterEffect());
    }

    private void HideBothPanels()
    {
        leftPanel?.SetActive(false);
        rightPanel?.SetActive(false);
    }

    //===========================================
    // TYPEWRITER
    //===========================================

    private IEnumerator TypewriterEffect()
    {
        isTyping = true;
        activeText.text = "";

        for (int i = 0; i <= fullText.Length; i++)
        {
            activeText.text = fullText.Substring(0, i);
            yield return new WaitForSeconds(1f / charactersPerSecond);
        }

        isTyping = false;
        typewriterCoroutine = null;

        // Handle auto-advance
        float autoTime = currentDialogue.Lines[currentLineIndex].autoAdvanceTime;
        if (autoTime > 0f)
        {
            autoAdvanceCoroutine = StartCoroutine(AutoAdvance(autoTime));
        }
    }

    private void CompleteTypewriter()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        activeText.text = fullText;
        isTyping = false;

        // Handle auto-advance
        float autoTime = currentDialogue.Lines[currentLineIndex].autoAdvanceTime;
        if (autoTime > 0f)
        {
            autoAdvanceCoroutine = StartCoroutine(AutoAdvance(autoTime));
        }
    }

    private IEnumerator AutoAdvance(float delay)
    {
        yield return new WaitForSeconds(delay);
        autoAdvanceCoroutine = null;
        AdvanceLine();
    }
}