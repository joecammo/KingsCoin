using UnityEngine;
using System.Collections;

public class WebLinkFade : MonoBehaviour
{
    private bool triggered = false;
    public float fadeDuration = 1.5f;

    // Optional: reference to SpriteRenderer (for 2D) or Renderer (for 3D)
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TriggerFadeAndDisable()
    {
        if (!triggered)
        {
            triggered = true;
            StartCoroutine(FadeAndDisable());
        }
    }

    private IEnumerator FadeAndDisable()
    {
        if (spriteRenderer != null)
        {
            Color startColor = spriteRenderer.color;
            float timer = 0f;
            while (timer < fadeDuration)
            {
                float t = timer / fadeDuration;
                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startColor.a, 0f, t));
                timer += Time.deltaTime;
                yield return null;
            }
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        }
        // Optionally: disable collider or destroy object
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        // Optionally: Destroy(gameObject);
    }

    public bool HasBeenTriggered() => triggered;
}
