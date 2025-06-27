using UnityEngine;

public class PlatformCollisionHelper : MonoBehaviour
{
    [HideInInspector]
    public RisingPlatformZone zone;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (zone != null && collision.gameObject.CompareTag("Player"))
        {
            zone.SetPlayerOnPlatform(true);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (zone != null && collision.gameObject.CompareTag("Player"))
        {
            zone.SetPlayerOnPlatform(false);
        }
    }
}
