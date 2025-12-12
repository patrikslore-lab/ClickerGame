using System.Collections.Generic;
using UnityEngine;

public class SelectiveColorManager : MonoBehaviour
{
    public static SelectiveColorManager Instance { get; private set; }

    [SerializeField, Range(0.001f, 0.1f)] private float tolerance = 0.01f;

    [Header("Active Pass-Through Colors")]
    [SerializeField] private List<Color> passColors = new List<Color>();

    private const int MAX_COLORS = 16;

    private static readonly int PassColorsID = Shader.PropertyToID("_PassColors");
    private static readonly int PassColorCountID = Shader.PropertyToID("_PassColorCount");
    private static readonly int ToleranceID = Shader.PropertyToID("_Tolerance");

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateMaterialProperties(Material material)
    {
        if (material == null) return;

        Vector4[] colorArray = new Vector4[MAX_COLORS];
        int count = Mathf.Min(passColors.Count, MAX_COLORS);

        for (int i = 0; i < count; i++)
        {
            colorArray[i] = passColors[i];
        }

        material.SetVectorArray(PassColorsID, colorArray);
        material.SetInt(PassColorCountID, count);
        material.SetFloat(ToleranceID, tolerance);

        // Debug logging
        if (count > 0)
        {
            Debug.Log($"SelectiveColorManager: Passing {count} colors to shader with tolerance {tolerance}");
            for (int i = 0; i < count; i++)
            {
                Debug.Log($"  Color {i}: RGB({passColors[i].r:F3}, {passColors[i].g:F3}, {passColors[i].b:F3})");
            }
        }
    }
    
    public void AddColor(Color color)
    {
        if (passColors.Count >= MAX_COLORS) return;

        if (!passColors.Contains(color))
        {
            passColors.Add(color);
        }
    }

    public void RemoveColor(Color color)
    {
        passColors.Remove(color);
    }

    public void ClearColors()
    {
        passColors.Clear();
    }
}