using UnityEngine;
using UnityEngine.UI;

public partial class SimplePlayerMovement : MonoBehaviour
{
    private float moveSpeed = 5f;
    private float jumpForce = 5f;
    private float slideSpeed = 3f;      // Speed for sliding down walls
    private LayerMask groundLayer;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded = false;
    private bool isWallSliding = false;
    private bool hasCollider = false;
    private bool hasRigidbody = false;
    private bool isFacingRight = true;  // Track which direction the character is facing
    public GameOverUI gameOverUI;
    public WallDetector wallDetector; // Reference to the wall detector
    private bool hasShownGameOver = false;

    // Moving platform tracking
    private Transform currentPlatform = null;
    private Vector3 lastPlatformPosition;


    // Coyote time variables
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter = 0f;

    // For smooth grounded animation
    private bool animatorIsGrounded = false;

    // Audio
    private AudioSource audioSource;

    // SFX
    public AudioClip platformSFX;
    public AudioClip coinSFX;
    private AudioSource sfxSource;
    private float platformSFXLastPlayTime = -Mathf.Infinity;
    private float platformSFXCooldown = 5f;

    // Web link
    public string webLinkURL = "https://your-url-here.com";

    // Victory UI
    public TMPro.TextMeshProUGUI victoryText;

    void Start()
    {
        // Check for required components
        hasCollider = GetComponent<Collider2D>() != null;
        hasRigidbody = GetComponent<Rigidbody2D>() != null;
        animator = GetComponent<Animator>();
        
        if (!hasCollider)
        {
            Debug.LogError("Player needs a Collider2D component!");
        }
        
        if (!hasRigidbody)
        {
            Debug.LogError("Player needs a Rigidbody2D component!");
        }
        
        if (animator == null)
        {
            Debug.LogError("Player needs an Animator component!");
        }
        

        
        rb = GetComponent<Rigidbody2D>();
        // Prevent Z-axis rotation
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        Debug.Log("Components initialized");

        // Hide Victory text at start
        if (victoryText != null)
            victoryText.gameObject.SetActive(false);

        // Get AudioSource
        audioSource = GetComponent<AudioSource>();

        // Add SFX AudioSource
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Continuously check if we're on a platform (handles moving platforms)
        if (collision.gameObject.CompareTag("Platform"))
        {
            isGrounded = true;
        }
    }

    void Update()
    {
        // Apply platform delta movement if on a moving platform
        if (currentPlatform != null)
        {
            Vector3 platformDelta = currentPlatform.position - lastPlatformPosition;
            transform.position += platformDelta;
            lastPlatformPosition = currentPlatform.position;
        }
        float move = Input.GetAxisRaw("Horizontal");

        // Clamp move to zero if nearly zero (prevents jitter)
        if (Mathf.Abs(move) < 0.01f) move = 0f;

        // Snap velocity.x to zero when grounded and not moving
        if (isGrounded && Mathf.Abs(move) < 0.01f)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        // COYOTE TIME update
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Disable only the direction that is blocked
        if (wallDetector != null)
        {
            if (move > 0 && wallDetector.disableRightInput)
            {
                move = 0f;
            }
            else if (move < 0 && wallDetector.disableLeftInput)
            {
                move = 0f;
            }
        }
        
        // Handle movement
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);
        
        // Handle facing direction
        if (move > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (move < 0 && isFacingRight)
        {
            Flip();
        }

        // Handle jumping with coyote time
        if (Input.GetKeyDown(KeyCode.Space) && coyoteTimeCounter > 0f)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            if (audioSource != null)
                audioSource.Play();
            AudioManager.Instance?.PlayJumpSound();
            coyoteTimeCounter = 0f; // Prevent double jump within coyote window
        }

        // Set YSpeed for Blend Tree (Jump/Fall), mask to 0 when grounded
        animator.SetFloat("YSpeed", isGrounded ? 0f : rb.linearVelocity.y);

        // Use coyote time for animator grounded state to prevent flicker
        animatorIsGrounded = isGrounded || coyoteTimeCounter > 0f;

        // Update animator
        animator.SetFloat("Speed", Mathf.Abs(move));
        animator.SetBool("IsGrounded", animatorIsGrounded);
        
        // Handle facing direction
        if (move > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (move < 0 && isFacingRight)
        {
            Flip();
        }
        
        // Handle jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            AudioManager.Instance?.PlayJumpSound();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we're colliding with any platform (including moving ones)
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("PlatformAlpha"))
        {
            isGrounded = true;
            Debug.Log("Player grounded on Platform or PlatformAlpha");
            // Track the platform for manual movement
            currentPlatform = collision.transform;
            lastPlatformPosition = currentPlatform.position;

            // Play SFX only for 'Platform'
            if (platformSFX != null && sfxSource != null && Time.time - platformSFXLastPlayTime >= platformSFXCooldown)
            {
                sfxSource.PlayOneShot(platformSFX);
                platformSFXLastPlayTime = Time.time;
            }
        }
        // Check if we're colliding with any jumpable platform
        if (collision.gameObject.CompareTag("Ground") || 
            collision.gameObject.CompareTag("StartingPlatform") || 
            collision.gameObject.CompareTag("Platform") || 
            collision.gameObject.CompareTag("PlatformAlpha"))
        {
            isGrounded = true;
            Debug.Log("Grounded on: " + collision.gameObject.name);
            
            // Only show game over prompt for ground collisions
            if (collision.gameObject.CompareTag("Ground") && !hasShownGameOver)
            {
                hasShownGameOver = true;
                if (gameOverUI != null)
                {
                    gameOverUI.ShowGameOver();
                }
            }
        }

        // Keep the player upright
        transform.rotation = Quaternion.identity;
    }

    private void Flip()
    {
        // Switch the way the player is facing
        isFacingRight = !isFacingRight;
        
        // Multiply the local scale x by -1 to flip the sprite
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        // Notify WallDetector of facing direction change
        if (wallDetector != null)
        {
            wallDetector.SetFacingDirection(isFacingRight);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Check if we're no longer colliding with any platform
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("PlatformAlpha"))
        {
            isGrounded = false;
            Debug.Log("No longer grounded on Platform or PlatformAlpha");
            // Clear reference to the platform
            currentPlatform = null;
        }

        // Keep the player upright
        transform.rotation = Quaternion.identity;
    }

    // Show Victory message when player touches Bitcoin
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bitcoin"))
        {
            if (victoryText != null)
            {
                victoryText.gameObject.SetActive(true);
            }
            // Play coin SFX
            if (coinSFX != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(coinSFX);
            }
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("WebLink"))
        {
            var fade = other.GetComponent<WebLinkFade>();
            if (fade != null && !fade.HasBeenTriggered())
            {
                if (!string.IsNullOrEmpty(webLinkURL))
                {
                    Application.OpenURL(webLinkURL);
                }
                fade.TriggerFadeAndDisable();
            }
            // If no fade script, fallback to destroying:
            // else if (fade == null) Destroy(other.gameObject);
        }
    }
}
