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
        if (playerInZone && playerOnPlatform && risenDistance < maxRiseDistance)
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
}

