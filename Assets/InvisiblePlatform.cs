using UnityEngine;

public class InvisiblePlatform : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool isFading = false;
    private bool isCountingDown = false;
    private float fadeSpeed = 2f;
    private Color originalColor;
    private bool hasAppeared = false;
    private float countdownTime = 2f;
    private float currentCountdownTime;
    private bool shouldHide = false;
    private float targetAlpha = 1f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("Platform needs a SpriteRenderer component!");
            return;
        }

        // Set target alpha based on tag
        if (CompareTag("PlatformAlpha"))
        {
            targetAlpha = 80f / 255f;
        }
        else
        {
            targetAlpha = 1f;
        }
        
        // Store the original color and make it invisible
        originalColor = spriteRenderer.color;
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }

    void Update()
    {
        if (isFading)
        {
            // Get the current color
            Color currentColor = spriteRenderer.color;

            // Increase alpha towards targetAlpha
            float newAlpha = Mathf.MoveTowards(currentColor.a, targetAlpha, fadeSpeed * Time.deltaTime);

            // Set new color with updated alpha
            spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);

            // Stop fading when alpha reaches targetAlpha
            if (Mathf.Approximately(newAlpha, targetAlpha) || newAlpha >= targetAlpha)
            {
                isFading = false;
                Debug.Log("Platform fully appeared!");
                StartCountdown();
            }
        }

        if (isCountingDown)
        {
            currentCountdownTime -= Time.deltaTime;
            
            // When time reaches 0
            if (currentCountdownTime <= 0)
            {
                isCountingDown = false;
                shouldHide = true;
            }
        }

        // Handle hiding the platform
        if (shouldHide)
        {
            // Start fading out
            Color currentColor = spriteRenderer.color;
            float newAlpha = Mathf.MoveTowards(currentColor.a, 0f, fadeSpeed * Time.deltaTime);
            spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            
            if (newAlpha <= 0f)
            {
                hasAppeared = false;
                shouldHide = false;
                Debug.Log("Platform has disappeared!");
            }
        }
    }

    private void StartCountdown()
    {
        isCountingDown = true;
        currentCountdownTime = countdownTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Platform collided with: " + collision.gameObject.name + " (Tag: " + collision.gameObject.tag + ")");
        
        // Check if the colliding object is the player
        if (collision.gameObject.CompareTag("Player") && !hasAppeared)
        {
            Debug.Log("Player detected! Starting fade in...");
            
            // Start the fade in
            isFading = true;
            hasAppeared = true;
            
            // Make the sprite visible
            spriteRenderer.enabled = true;
            
            // Debug log to confirm appearance
            Debug.Log("Platform starting to appear!");
        }
    }
}
