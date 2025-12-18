// GlobalColorPreservation.cs
using UnityEngine;

public class GlobalColorPreservation : MonoBehaviour
{
    public static GlobalColorPreservation Instance { get; private set; }
    
    [Header("Color Preservation Controls")]
    [Tooltip("Check a color index to PRESERVE it (keep in color), uncheck to DESATURATE it")]
    [SerializeField] private bool color_00;
    [SerializeField] private bool color_01;
    [SerializeField] private bool color_02;
    [SerializeField] private bool color_03;
    [SerializeField] private bool color_04;
    [SerializeField] private bool color_05;
    [SerializeField] private bool color_06;
    [SerializeField] private bool color_07;
    [SerializeField] private bool color_08;
    [SerializeField] private bool color_09;
    [SerializeField] private bool color_10;
    [SerializeField] private bool color_11;
    [SerializeField] private bool color_12;
    [SerializeField] private bool color_13;
    [SerializeField] private bool color_14;
    [SerializeField] private bool color_15;
    [SerializeField] private bool color_16;
    [SerializeField] private bool color_17;
    [SerializeField] private bool color_18;
    [SerializeField] private bool color_19;
    [SerializeField] private bool color_20;
    [SerializeField] private bool color_21;
    [SerializeField] private bool color_22;
    [SerializeField] private bool color_23;
    [SerializeField] private bool color_24;
    [SerializeField] private bool color_25;
    [SerializeField] private bool color_26;
    [SerializeField] private bool color_27;
    [SerializeField] private bool color_28;
    [SerializeField] private bool color_29;
    [SerializeField] private bool color_30;
    [SerializeField] private bool color_31;
    [SerializeField] private bool color_32;
    [SerializeField] private bool color_33;
    [SerializeField] private bool color_34;
    [SerializeField] private bool color_35;
    [SerializeField] private bool color_36;
    [SerializeField] private bool color_37;
    [SerializeField] private bool color_38;
    [SerializeField] private bool color_39;
    [SerializeField] private bool color_40;
    [SerializeField] private bool color_41;
    [SerializeField] private bool color_42;
    [SerializeField] private bool color_43;
    [SerializeField] private bool color_44;
    [SerializeField] private bool color_45;
    [SerializeField] private bool color_46;
    [SerializeField] private bool color_47;
    [SerializeField] private bool color_48;
    [SerializeField] private bool color_49;
    [SerializeField] private bool color_50;
    [SerializeField] private bool color_51;
    
    private Texture2D preservationMaskTexture;
    private Color[] maskColors = new Color[52];
    private static readonly int PreservationMaskID = Shader.PropertyToID("_PreservationMask");
    
    private bool[] previousStates = new bool[52];
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        
        // Create texture
        CreatePreservationTexture();
    }
    
    void CreatePreservationTexture()
    {
        try
        {
            preservationMaskTexture = new Texture2D(52, 1, TextureFormat.R8, false);
            preservationMaskTexture.filterMode = FilterMode.Point;
            preservationMaskTexture.wrapMode = TextureWrapMode.Clamp;
            preservationMaskTexture.name = "PreservationMask";
            
            Debug.Log("✓ Preservation texture created successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create preservation texture: {e.Message}");
        }
    }
    
    void Start()
    {
        if (preservationMaskTexture == null)
        {
            Debug.LogError("Texture was null in Start! Recreating...");
            CreatePreservationTexture();
        }
        
        CopyCheckboxesToArray();
        UpdatePreservationMask();
    }
    
    void Update()
    {
        if (preservationMaskTexture == null)
        {
            Debug.LogError("Texture became null during runtime!");
            return;
        }
        
        // Check if any checkbox changed
        bool changed = false;
        bool[] currentStates = GetCheckboxStates();
        
        for (int i = 0; i < 52; i++)
        {
            if (currentStates[i] != previousStates[i])
            {
                changed = true;
                previousStates[i] = currentStates[i];
            }
        }
        
        if (changed)
        {
            UpdatePreservationMask();
        }
    }
    
    private void CopyCheckboxesToArray()
    {
        bool[] states = GetCheckboxStates();
        for (int i = 0; i < 52; i++)
        {
            previousStates[i] = states[i];
        }
    }
    
    private bool[] GetCheckboxStates()
    {
        return new bool[]
        {
            color_00, color_01, color_02, color_03, color_04, color_05, color_06, color_07,
            color_08, color_09, color_10, color_11, color_12, color_13, color_14, color_15,
            color_16, color_17, color_18, color_19, color_20, color_21, color_22, color_23,
            color_24, color_25, color_26, color_27, color_28, color_29, color_30, color_31,
            color_32, color_33, color_34, color_35, color_36, color_37, color_38, color_39,
            color_40, color_41, color_42, color_43, color_44, color_45, color_46, color_47,
            color_48, color_49, color_50, color_51
        };
    }
    
    private void UpdatePreservationMask()
    {
        if (preservationMaskTexture == null)
        {
            Debug.LogError("Cannot update mask - texture is null!");
            return;
        }
        
        bool[] states = GetCheckboxStates();
        
        for (int i = 0; i < 52; i++)
        {
            // Checked = preserve (white), unchecked = desaturate (black)
            maskColors[i] = states[i] ? Color.white : Color.black;
        }
        
        ApplyMaskTexture();
    }
    
    private void ApplyMaskTexture()
    {
        if (preservationMaskTexture == null)
        {
            Debug.LogError("Cannot apply mask - texture is null!");
            return;
        }
        
        try
        {
            preservationMaskTexture.SetPixels(maskColors);
            preservationMaskTexture.Apply();
            
            // Find all sprites using our shader graph material and update the PreservationMask texture
            SpriteRenderer[] allSprites = FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
            int updateCount = 0;
            
            foreach (var sr in allSprites)
            {
                if (sr.sharedMaterial != null && 
                    (sr.sharedMaterial.shader.name.Contains("ColorPreservingSpriteLit") ||
                     sr.sharedMaterial.shader.name.Contains("ColorPreservingSprite")))
                {
                    // Set the texture on the material instance
                    sr.material.SetTexture("_PreservationMask", preservationMaskTexture);
                    updateCount++;
                }
            }
            
            int preservedCount = 0;
            for (int i = 0; i < 52; i++)
            {
                if (maskColors[i] == Color.white)
                    preservedCount++;
            }
            
            Debug.Log($"✓ Mask applied to {updateCount} sprites: {preservedCount}/52 colors preserved");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to apply mask: {e.Message}");
        }
    }
    
    public void PreserveColor(int index, bool preserve)
    {
        if (index < 0 || index >= 52) return;
        
        switch (index)
        {
            case 0: color_00 = preserve; break;
            case 1: color_01 = preserve; break;
            case 2: color_02 = preserve; break;
            case 3: color_03 = preserve; break;
            case 4: color_04 = preserve; break;
            case 5: color_05 = preserve; break;
            case 6: color_06 = preserve; break;
            case 7: color_07 = preserve; break;
            case 8: color_08 = preserve; break;
            case 9: color_09 = preserve; break;
            case 10: color_10 = preserve; break;
            case 11: color_11 = preserve; break;
            case 12: color_12 = preserve; break;
            case 13: color_13 = preserve; break;
            case 14: color_14 = preserve; break;
            case 15: color_15 = preserve; break;
            case 16: color_16 = preserve; break;
            case 17: color_17 = preserve; break;
            case 18: color_18 = preserve; break;
            case 19: color_19 = preserve; break;
            case 20: color_20 = preserve; break;
            case 21: color_21 = preserve; break;
            case 22: color_22 = preserve; break;
            case 23: color_23 = preserve; break;
            case 24: color_24 = preserve; break;
            case 25: color_25 = preserve; break;
            case 26: color_26 = preserve; break;
            case 27: color_27 = preserve; break;
            case 28: color_28 = preserve; break;
            case 29: color_29 = preserve; break;
            case 30: color_30 = preserve; break;
            case 31: color_31 = preserve; break;
            case 32: color_32 = preserve; break;
            case 33: color_33 = preserve; break;
            case 34: color_34 = preserve; break;
            case 35: color_35 = preserve; break;
            case 36: color_36 = preserve; break;
            case 37: color_37 = preserve; break;
            case 38: color_38 = preserve; break;
            case 39: color_39 = preserve; break;
            case 40: color_40 = preserve; break;
            case 41: color_41 = preserve; break;
            case 42: color_42 = preserve; break;
            case 43: color_43 = preserve; break;
            case 44: color_44 = preserve; break;
            case 45: color_45 = preserve; break;
            case 46: color_46 = preserve; break;
            case 47: color_47 = preserve; break;
            case 48: color_48 = preserve; break;
            case 49: color_49 = preserve; break;
            case 50: color_50 = preserve; break;
            case 51: color_51 = preserve; break;
        }
        
        UpdatePreservationMask();
    }
    
    void OnDestroy()
    {
        if (preservationMaskTexture != null)
            Destroy(preservationMaskTexture);
    }
}