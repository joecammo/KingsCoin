using UnityEngine;

public class PlatformFallTriggerWebLink : MonoBehaviour
{
    public string groundTag = "Ground";
    public string webLinkTag = "WebLink";
    public string webLinkObjectName = "bitcoinwallet2_0";

    private bool triggered = false;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!triggered && collision.gameObject.CompareTag(groundTag))
        {
            triggered = true;
            // Find the WebLink object by tag and name
            GameObject[] webLinks = GameObject.FindGameObjectsWithTag(webLinkTag);
            foreach (GameObject obj in webLinks)
            {
                if (obj.name == webLinkObjectName)
                {
                    Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                    if (rb == null)
                    {
                        rb = obj.AddComponent<Rigidbody2D>();
                    }
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    rb.constraints = RigidbodyConstraints2D.None;
                    rb.gravityScale = 1f;
                    // Do not destroy the object; it remains interactable
                    Debug.Log($"[PlatformFallTriggerWebLink] {webLinkObjectName} now falls with physics!");

                    // Shake the background
                    GameObject bg = GameObject.Find("BG1");
                    if (bg != null)
                    {
                        BackgroundShake shaker = bg.GetComponent<BackgroundShake>();
                        if (shaker != null)
                            shaker.Shake();
                    }
                }
            }
        }
    }
}
