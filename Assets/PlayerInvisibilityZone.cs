using UnityEngine;

public class PlayerInvisibilityZone : MonoBehaviour
{
    // This script should be placed on the invisible zone object (with a Collider2D set as Trigger)
    // The player must have a SpriteRenderer

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SpriteRenderer sr = other.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sr.color;
                c.a = 0f;
                sr.color = c;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SpriteRenderer sr = other.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sr.color;
                c.a = 1f;
                sr.color = c;
            }
        }
    }
}
