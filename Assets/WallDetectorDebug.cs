using UnityEngine;

public class WallDetectorDebug : MonoBehaviour
{
    public Color normalColor = new Color(0, 0, 1, 0.5f);  // Semi-transparent blue
    public Color detectedColor = new Color(1, 0, 0, 0.5f);  // Semi-transparent red
    public SpriteRenderer spriteRenderer;
    public WallDetector wallDetector;

    void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("WallDetectorDebug needs a SpriteRenderer component!");
            }
        }

        if (wallDetector == null)
        {
            Debug.LogError("WallDetectorDebug needs a reference to WallDetector!");
        }
    }

    void Update()
    {
        // Update color based on detection state
        Color targetColor = wallDetector.isWallDetected ? detectedColor : normalColor;
        if (spriteRenderer.color != targetColor)
        {
            spriteRenderer.color = targetColor;
        }
    }
}
