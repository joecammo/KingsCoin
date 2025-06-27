using UnityEngine;

public class RisingPlatformJumpCounter : MonoBehaviour
{
    public RisingPlatformZone zone; // Assign in Inspector or by script
    public string risingPlatformTag = "Platform"; // Tag for the rising platform
    public string playerTag = "Player";
    public int jumpsRequired = 3;

    private int jumpCount = 0;
    private bool playerInZone = false;
    private bool playerOnRisingPlatform = false;
    private bool playerWasGrounded = false;
    private SimplePlayerMovement playerMovement;
    private Transform risingPlatformTransform;

    void Start()
    {
        if (zone == null)
        {
            Debug.LogError("RisingPlatformZone reference not set!");
            enabled = false;
            return;
        }
        risingPlatformTransform = zone.platform;
    }

    void Update()
    {
        if (playerInZone && playerMovement != null)
        {
            bool isGrounded = playerMovementIsGrounded();
            if (isGrounded && !playerWasGrounded && playerOnRisingPlatform)
            {
                jumpCount++;
                Debug.Log($"[RisingPlatformJumpCounter] Jumped on rising platform. Count: {jumpCount}");
                if (jumpCount >= jumpsRequired)
                {
                    jumpCount = 0;
                    zone.TriggerPlatformFall();
                }
            }
            playerWasGrounded = isGrounded;
        }
    }

    private bool playerMovementIsGrounded()
    {
        return playerMovement != null && (bool)playerMovement.GetType().GetField("isGrounded", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(playerMovement);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInZone = true;
            playerMovement = other.GetComponent<SimplePlayerMovement>();
        }
        if (other.transform == risingPlatformTransform)
        {
            playerOnRisingPlatform = true;
        }
        else if (playerInZone && other.CompareTag("Platform") && other.transform != risingPlatformTransform)
        {
            // Landed on another platform inside the zone, reset
            jumpCount = 0;
            Debug.Log("[RisingPlatformJumpCounter] Landed on another platform. Counter reset to 0.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInZone = false;
            playerMovement = null;
            jumpCount = 0;
        }
        if (other.transform == risingPlatformTransform)
        {
            playerOnRisingPlatform = false;
        }
    }
}
