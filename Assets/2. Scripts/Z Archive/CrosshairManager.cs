using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    // References to the UI components we'll need
    private Image crosshairImage;  // This will display our crosshair texture
    private RectTransform crosshairRect;  // This controls the position and size

    // Crosshair settings (you can change these in the Inspector)
    [SerializeField] private float baseSize = 50f;  // Default size of the crosshair
    private float currentSize;  // The current size (can change during gameplay)

    void Start()
    {
        // Get references to the components on this GameObject
        crosshairImage = GetComponent<Image>();
        crosshairRect = GetComponent<RectTransform>();

        // Set the initial size
        currentSize = baseSize;
        UpdateCrosshairSize();
        
        // Hide the default mouse cursor since we're drawing our own
        Cursor.visible = false;
    }

    void Update()
    {
        // Update the crosshair position to follow the mouse every frame
        UpdateCrosshairPosition();
    }

    /// <summary>
    /// Moves the crosshair to the current mouse position
    /// </summary>
    void UpdateCrosshairPosition()
    {
        // Get the mouse position in screen space
        Vector3 mousePos = Input.mousePosition;

        // Set the crosshair position to match the mouse
        crosshairRect.position = mousePos;
    }

    /// <summary>
    /// Changes the size of the crosshair
    /// You can call this from other scripts to make the crosshair bigger or smaller
    /// </summary>
    public void SetCrosshairSize(float newSize)
    {
        currentSize = newSize;
        UpdateCrosshairSize();
    }

    /// <summary>
    /// Helper method that actually applies the size to the crosshair
    /// </summary>
    void UpdateCrosshairSize()
    {
        // Set the crosshair to the new size using sizeDelta
        // This controls the width and height of the image
        crosshairRect.sizeDelta = new Vector2(currentSize, currentSize);
    }

    /// <summary>
    /// Returns the current size so other scripts can check it
    /// </summary>
    public float GetCrosshairSize()
    {
        return currentSize;
    }
}