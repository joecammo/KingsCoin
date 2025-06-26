using UnityEngine;

public class WallDetector : MonoBehaviour
{
    public bool isWallDetected = false;
    public bool isPlatform = false;
    public bool touchingPlatform = false; // True when touching a Platform
    public bool disableRightInput = false;
    public bool disableLeftInput = false;
    private bool isFacingRight = true;
    public Vector2 offset = new Vector2(0.5f, 0f);  // Default offset from player
    public Transform player; // Assign this in the Inspector

    void LateUpdate()
    {
        if (player != null)
        {
            float playerSign = Mathf.Sign(player.localScale.x);
            transform.position = player.position + new Vector3(offset.x * playerSign, offset.y, 0f);
        }
    }

    public void SetFacingDirection(bool facingRight)
    {
        isFacingRight = facingRight;
    }

    // SFX
    public AudioClip platformSFX;
    private AudioSource sfxSource;

    void Start()
    {
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("StartingPlatform"))
        {
            touchingPlatform = true;
            Debug.Log($"WallDetector OnTriggerEnter2D: position.x={transform.position.x}, touching {collision.gameObject.name}");

            // Play SFX only for 'Platform'
            if (collision.gameObject.CompareTag("Platform") && platformSFX != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(platformSFX);
            }

            if (player != null)
            {
                float diff = transform.position.x - player.position.x;
                if (diff > 0)
                {
                    disableRightInput = true;
                    Debug.Log("WallDetector: Disabling RIGHT input");
                }
                else if (diff < 0)
                {
                    disableLeftInput = true;
                    Debug.Log("WallDetector: Disabling LEFT input");
                }
                Debug.Log($"WallDetector flags after trigger: disableLeftInput={disableLeftInput}, disableRightInput={disableRightInput}");
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Platform") || collision.gameObject.CompareTag("StartingPlatform"))
        {
            touchingPlatform = false;
            disableRightInput = false;
            disableLeftInput = false;
            Debug.Log("WallDetector: Re-enabling both LEFT and RIGHT input");
        }
    }


}
