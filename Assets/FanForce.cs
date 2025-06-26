using UnityEngine;

public class FanForce : MonoBehaviour
{
    public float fanForce = 5f; // Adjust in Inspector

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(Vector2.up * fanForce, ForceMode2D.Force);
            }
        }
    }
}
