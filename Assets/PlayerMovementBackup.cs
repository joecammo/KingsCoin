using UnityEngine;

public class PlayerMovementBackup : MonoBehaviour
{
    private float moveSpeed = 5f;
    private float jumpForce = 10f;
    private LayerMask groundLayer;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool hasCollider = false;
    private bool hasRigidbody = false;

    void Start()
    {
        // Check for required components
        hasCollider = GetComponent<Collider2D>() != null;
        hasRigidbody = GetComponent<Rigidbody2D>() != null;
        
        if (!hasCollider)
        {
            Debug.LogError("Player needs a Collider2D component!");
        }
        
        if (!hasRigidbody)
        {
            Debug.LogError("Player needs a Rigidbody2D component!");
        }
        
        rb = GetComponent<Rigidbody2D>();
        Debug.Log("Components initialized");
    }

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");
        
        // Handle movement
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);
        
        // Handle jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we're colliding with ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("Grounded on: " + collision.gameObject.name);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Check if we're no longer colliding with ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("No longer grounded");
        }
    }
    }

