using UnityEngine;

public class PlatformResetListener : MonoBehaviour
{
    public RisingPlatformZone zone;

    void Start()
    {
        Debug.Log($"[PlatformResetListener] Started on {gameObject.name}. zone assigned: {zone != null}");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[PlatformResetListener] OnTriggerEnter2D: {gameObject.name} entered trigger with {other.gameObject.name}, tag: {other.tag}");
        if (zone != null && other.CompareTag("PlatformReset"))
        {
            Debug.Log("[PlatformResetListener] PlatformReset tag detected. Calling zone.TriggerPlatformFall()");
            zone.TriggerPlatformFall();
        }
        else if (zone == null)
        {
            Debug.LogWarning("[PlatformResetListener] zone reference is null!");
        }
    }
}
