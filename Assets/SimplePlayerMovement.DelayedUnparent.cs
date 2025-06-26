using System.Collections;
using UnityEngine;

public partial class SimplePlayerMovement : MonoBehaviour
{
    // Coroutine to delay unparenting by one frame
    private IEnumerator DelayedUnparent()
    {
        yield return null; // wait one frame
        transform.SetParent(null);
    }
}
