using UnityEngine;

public class RisingPlatformZone : MonoBehaviour
{
    public AudioClip platformYellowFirstSound;
    public AudioSource audioSource;
    private bool platformYellowSoundPlayed = false;

    [Header("Background Music")]
    public AudioClip backgroundMusic;
    public AudioSource backgroundMusicSource;
    public Transform platform; // Assign the platform inside the zone in Inspector
    public float riseSpeed = 1f;
    // public float maxRiseDistance = 20f;
    private float platformCeilingY; // The top Y of the zone
    private Vector3 initialPlatformPosition;
    private bool playerOnPlatform = false;
    private bool playerInZone = false;
    private float risenDistance = 0f;
    private bool falling = false;
    private bool shouldReturnToStart = false;

    void Start()
    {
        if (platform == null)
        {
            Debug.LogError("Assign the platform Transform in the Inspector!");
            enabled = false;
            return;
        }
        initialPlatformPosition = platform.position;

        // Calculate the top Y of the zone collider (assumes BoxCollider2D on this GameObject)
        BoxCollider2D zoneCollider = GetComponent<BoxCollider2D>();
        if (zoneCollider != null)
        {
            float zoneTop = zoneCollider.bounds.max.y;
            platformCeilingY = zoneTop;
        }
        else
        {
            Debug.LogWarning("No BoxCollider2D found on zone for ceiling calculation.");
            platformCeilingY = initialPlatformPosition.y + 20f; // fallback
        }

        // Play background music if assigned
        if (backgroundMusicSource != null && backgroundMusic != null)
        {
            backgroundMusicSource.clip = backgroundMusic;
            backgroundMusicSource.loop = true;
            backgroundMusicSource.Play();
        }
    }

    void Update()
    {
        // Prevent errors if the platform has been destroyed
        if (platform == null) return;
        // If falling is triggered externally, platform will fall in Update

        if (falling)
        {
            // Fall down until reaching initial position
            float fallStep = riseSpeed * 1.5f * Time.deltaTime; // Falls a bit faster than rise
            Vector3 target = initialPlatformPosition;
            if (platform.position.y > target.y)
            {
                float newY = Mathf.MoveTowards(platform.position.y, target.y, fallStep);
                platform.position = new Vector3(platform.position.x, newY, platform.position.z);
                risenDistance = Mathf.Max(0f, risenDistance - (platform.position.y - newY));
            }
            if (Mathf.Approximately(platform.position.y, target.y))
            {
                platform.position = target;
                risenDistance = 0f;
                falling = false;
            }
        }
        else if (shouldReturnToStart && !falling)
        {
            float returnStep = riseSpeed * Time.deltaTime;
            Vector3 target = initialPlatformPosition;
            if (platform.position.y > target.y)
            {
                float newY = Mathf.MoveTowards(platform.position.y, target.y, returnStep);
                platform.position = new Vector3(platform.position.x, newY, platform.position.z);
                risenDistance = Mathf.Max(0f, risenDistance - (platform.position.y - newY));
            }
            if (Mathf.Approximately(platform.position.y, target.y))
            {
                platform.position = target;
                risenDistance = 0f;
                shouldReturnToStart = false;
            }
        }
        else if (playerInZone && playerOnPlatform && platform.position.y < platformCeilingY)
        {
            float riseStep = riseSpeed * Time.deltaTime;
            float newY = Mathf.Min(platform.position.y + riseStep, platformCeilingY);
            platform.position = new Vector3(platform.position.x, newY, platform.position.z);
            risenDistance = Mathf.Max(0f, platform.position.y - initialPlatformPosition.y);
        }
    }

    // Zone trigger logic
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
        }
        if (other.gameObject.name == "PlatformYellow (18)")
        {
            Debug.Log("Triggered with PlatformYellow (18)!");
            UIFadeInText fadeText = Object.FindFirstObjectByType<UIFadeInText>();
            if (fadeText != null)
                fadeText.FadeInAndOut();
            else
                Debug.LogWarning("No UIFadeInText found in scene!");

            // Play sound only the first time
            if (!platformYellowSoundPlayed && audioSource != null && platformYellowFirstSound != null)
            {
                audioSource.PlayOneShot(platformYellowFirstSound);
                platformYellowSoundPlayed = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            if (!falling)
            {
                shouldReturnToStart = true;
            }
        }
    }

    // Platform collision logic
    void OnEnable()
    {
        if (platform != null)
        {
            var col = platform.GetComponent<Collider2D>();
            if (col != null)
            {
                PlatformCollisionHelper helper = platform.GetComponent<PlatformCollisionHelper>();
                if (helper == null)
                    platform.gameObject.AddComponent<PlatformCollisionHelper>().zone = this;
                else
                    helper.zone = this;
            }
        }
    }



    // Called by PlatformCollisionHelper
    public void SetPlayerOnPlatform(bool onPlatform)
    {
        playerOnPlatform = onPlatform;
    }

    // Called by PlatformResetListener to trigger the platform to fall
    public void TriggerPlatformFall()
    {
        Debug.Log($"[RisingPlatformZone] TriggerPlatformFall CALLED. falling={{falling}}, platform.position={{platform.position}}, initialPlatformPosition={{initialPlatformPosition}}, stackTrace={{System.Environment.StackTrace}} ");
        if (!falling && platform.position.y > initialPlatformPosition.y + 0.01f)
        {
            Debug.Log("[RisingPlatformZone] TriggerPlatformFall: Falling triggered.");
            falling = true;

            // Enable full physics drop
            Rigidbody2D rb = platform.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.constraints = RigidbodyConstraints2D.None;
                rb.gravityScale = 1f; // Ensure normal gravity
            }

            // Optionally destroy the platform after 3 seconds (remove if you want it to persist)
            Destroy(platform.gameObject, 3f);
        }
        else
        {
            Debug.Log("[RisingPlatformZone] TriggerPlatformFall: Ignored, already falling or at initial position.");
        }
    }
}

