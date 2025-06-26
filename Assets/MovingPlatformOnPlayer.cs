using UnityEngine;

public class MovingPlatformOnPlayer : MonoBehaviour
{
    public float moveDistance = 2f; // How far up and down to move
    public float moveSpeed = 2f;    // How fast to move
    private Vector3 startPos;
    private bool playerOnPlatform = false;
    private float direction = 1f;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (playerOnPlatform)
        {
            float newY = transform.position.y + direction * moveSpeed * Time.deltaTime;
            float upperLimit = startPos.y + moveDistance;
            float lowerLimit = startPos.y - moveDistance;

            // Reverse direction at limits
            if (newY > upperLimit)
            {
                newY = upperLimit;
                direction = -1f;
            }
            else if (newY < lowerLimit)
            {
                newY = lowerLimit;
                direction = 1f;
            }

            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = false;
        }
    }
}
