using UnityEngine;

public class RisingPlatformZone : MonoBehaviour
{
    public Transform platform; // Assign the platform inside the zone in Inspector
    public float riseSpeed = 1f;
    public float maxRiseDistance = 5f;
    private Vector3 initialPlatformPosition;
    private bool playerOnPlatform = false;
    private bool playerInZone = false;
    private float risenDistance = 0f;
    private bool falling = false;

    void Start()
    {
        if (platform == null)
        {
            Debug.LogError("Assign the platform Transform in the Inspector!");
            enabled = false;
            return;
        }
        initialPlatformPosition = platform.position;
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
        else if (playerInZone && playerOnPlatform && risenDistance < maxRiseDistance)
        {
            float riseStep = riseSpeed * Time.deltaTime;
            platform.position += new Vector3(0, riseStep, 0);
            risenDistance += riseStep;
        }
    }

    // Zone trigger logic
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
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

