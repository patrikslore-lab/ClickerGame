using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject gameplayPanel;
    public GameObject levelCompletionPanel;
    public GameObject pauseMenuPanel;
    public GameObject gameOverPanel;
    public GameObject basePanel;
    public GameObject mainMenuPanel;
    public GameObject startGamePanel;
    public GameObject upgradePanel;

    [SerializeField] private TextMeshProUGUI woodCountTextBox;

    [SerializeField] private TextMeshProUGUI coreCountTextBox;

    [SerializeField] public TextMeshProUGUI juneCooldownTextBox;

    [SerializeField] private Image RicochetOnImage;
    [SerializeField] private Image RicochetCooldownImage;
    [SerializeField] private Image RicochetAvailableImage;

    [SerializeField] private Image LooterOnImage;
    [SerializeField] private Image LooterCooldownImage;
    [SerializeField] private Image LooterAvailableImage;

    [SerializeField] private Image ProtectorOnImage;
    [SerializeField] private Image ProtectorCooldownImage;
    [SerializeField] private Image ProtectorAvailableImage;

    [Header("Grade Popup System")]
    [SerializeField] private bool enableGradePopups = true;
    [SerializeField] private GameObject gradePopupPrefab;
    [SerializeField] private Sprite sRankSprite;
    [SerializeField] private Sprite aRankSprite;
    [SerializeField] private Sprite bRankSprite;
    [SerializeField] private Sprite cRankSprite;
    [SerializeField] private Sprite dRankSprite;

    private PlayerConfig playerConfig;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is null in UIManager.Start()!");
            return;
        }

        playerConfig = GameManager.Instance.GetPlayerConfig();
        if (playerConfig != null)
        {
            UpdateWoodCountUI(playerConfig.wood);
            UpdateCoreCountUI(playerConfig.corePieces);
        }
        else
        {
            Debug.LogError("PlayerConfig is null! Assign it in GameManager Inspector.");
        }
    }

    void Update()
    {
        if (!CooldownController.Instance.IsOnCooldown)
        {
        juneCooldownTextBox.text = "READY";
        } 
        else
        {
        juneCooldownTextBox.text = $"{CooldownController.Instance.CooldownRemaining:F0}";
        }   
    }

    public void InitializePanels()
    {
        // Hide all panels initially - GameManager will set the correct mode/state
        HideAllPanels();
    }

    public void HideAllPanels()
    {
        gameplayPanel?.SetActive(false);
        levelCompletionPanel?.SetActive(false);
        pauseMenuPanel?.SetActive(false);
        gameOverPanel?.SetActive(false);
        basePanel?.SetActive(false);
        mainMenuPanel?.SetActive(false);
        upgradePanel?.SetActive(false);
        startGamePanel?.SetActive(false);
    }

    public void UpdateWoodCountUI(float totalWood)
    {
        if (woodCountTextBox != null)
            woodCountTextBox.text = $"{totalWood}";
    }
    public void UpdateCoreCountUI(float totalCores)
    {
        if (coreCountTextBox != null)
            coreCountTextBox.text = $"{totalCores}";
    }

    public void SpawnGradePopup(ReactionGrade.Grade grade, Vector3 worldPosition)
    {
        if (!enableGradePopups) return;

        if (gradePopupPrefab == null)
        {
            Debug.LogError("UIManager: gradePopupPrefab not assigned!");
            return;
        }

        Sprite gradeSprite = grade switch
        {
            ReactionGrade.Grade.S => sRankSprite,
            ReactionGrade.Grade.A => aRankSprite,
            ReactionGrade.Grade.B => bRankSprite,
            ReactionGrade.Grade.C => cRankSprite,
            ReactionGrade.Grade.D => dRankSprite,
            _ => null
        };

        if (gradeSprite == null)
        {
            Debug.LogWarning($"UIManager: No sprite assigned for grade {grade}");
            return;
        }

        GameObject popup = Instantiate(gradePopupPrefab, worldPosition, Quaternion.identity);
        GradePopup gradePopupComponent = popup.GetComponent<GradePopup>();
        if (gradePopupComponent != null)
        {
            gradePopupComponent.Initialize(gradeSprite, worldPosition);
        }
        else
        {
            Debug.LogError("UIManager: gradePopupPrefab is missing GradePopup component!");
            Destroy(popup);
        }
    }

    public void RicochetActivate()
    {
        RicochetOnImage.enabled = true;
        RicochetCooldownImage.enabled = false;
        RicochetAvailableImage.enabled = false;
    }

    public void RicochetOnCooldown()
    {
        RicochetOnImage.enabled = false;
        RicochetCooldownImage.enabled = true;
        RicochetAvailableImage.enabled = false;
    }

    public void RicochetAvailable()
    {
        RicochetOnImage.enabled = false;
        RicochetCooldownImage.enabled = false;
        RicochetAvailableImage.enabled = true;
    }

    public void LooterActivate()
    {
        if (LooterOnImage != null)
        {
            LooterOnImage.enabled = true;
            LooterCooldownImage.enabled = false;
            LooterAvailableImage.enabled = false;
        }
    }

    public void LooterOnCooldown()
    {
        if (LooterCooldownImage != null)
        {
            LooterOnImage.enabled = false;
            LooterCooldownImage.enabled = true;
            LooterAvailableImage.enabled = false;
        }
    }

    public void LooterAvailable()
    {
        if (LooterAvailableImage != null)
        {
            LooterOnImage.enabled = false;
            LooterCooldownImage.enabled = false;
            LooterAvailableImage.enabled = true;
        }
    }

    public void ProtectorActivate()
    {
        if (ProtectorOnImage != null)
        {
            ProtectorOnImage.enabled = true;
            ProtectorCooldownImage.enabled = false;
            ProtectorAvailableImage.enabled = false;
        }
    }

    public void ProtectorOnCooldown()
    {
        if (ProtectorCooldownImage != null)
        {
            ProtectorOnImage.enabled = false;
            ProtectorCooldownImage.enabled = true;
            ProtectorAvailableImage.enabled = false;
        }
    }

    public void ProtectorAvailable()
    {
        if (ProtectorAvailableImage != null)
        {
            ProtectorOnImage.enabled = false;
            ProtectorCooldownImage.enabled = false;
            ProtectorAvailableImage.enabled = true;
        }
    }

    private void OnEnable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnIntroPhase2_LightActivate += HandleIntroPhase2;
        }
    }

    private void OnDisable()
    {
        if (EventManager.Instance != null)
        {
            EventManager.Instance.OnIntroPhase2_LightActivate -= HandleIntroPhase2;
        }
    }
    private void HandleIntroPhase2()
    {
        StartCoroutine(RevealUISequence());
    }

    // NEW: Add this method
    private IEnumerator RevealUISequence()
    {
        // If you have a CanvasGroup on gameplay panel, fade it in
        CanvasGroup canvasGroup = gameplayPanel?.GetComponent<CanvasGroup>();
        
        if (canvasGroup != null)
        {
            float elapsed = 0f;
            float duration = 1f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
                yield return null;
            }
            
            canvasGroup.alpha = 1f;
        }
        else
        {
            // No fade, just show the panel immediately
            gameplayPanel?.SetActive(true);
        }
    }
}