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
        if (PlayerManager.Instance == null) return;

        if (!PlayerManager.Instance.IsOnCooldown)
        {
            juneCooldownTextBox.text = "READY";
        }
        else
        {
            juneCooldownTextBox.text = $"{PlayerManager.Instance.CooldownRemaining:F0}";
        }
    }

    public void ShowUpgradePanel()
    {
        if (upgradePanel.activeSelf)
        {
            upgradePanel.SetActive(false);
        }
        else
        {
            upgradePanel.SetActive(true);
        }
    }

    public void HideUpgradePanel()
    {
        upgradePanel.SetActive(false);
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

    /// Prepares UI for level intro sequence (panel visible but transparent)
    public void PrepareForLevelIntro()
    {
        HideAllPanels();
        
        if (gameplayPanel != null)
        {
            gameplayPanel.SetActive(true);
            
            var canvasGroup = gameplayPanel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
        }
    }
    public void ShowGameplayUI()
    {
        HideAllPanels();
        
        if (gameplayPanel != null)
        {
            gameplayPanel.SetActive(true);
            
            var canvasGroup = gameplayPanel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }
    }
    /// Coroutine to fade in gameplay UI (for intro sequence)
    public IEnumerator RevealGameplayUI(float duration = 1f)
    {
        if (gameplayPanel == null) yield break;
        
        var canvasGroup = gameplayPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) yield break;
        
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
}