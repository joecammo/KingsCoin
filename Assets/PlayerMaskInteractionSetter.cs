using UnityEngine;

public class PlayerMaskInteractionSetter : MonoBehaviour
{
    void Awake()
    {
        // Set all SpriteRenderers on this GameObject and its children to Visible Outside Mask
        var renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in renderers)
        {
            sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }
    }
}
