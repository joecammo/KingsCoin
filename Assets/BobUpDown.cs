using UnityEngine;

public class BobUpDown : MonoBehaviour
{
    public float amplitude = 0.25f;  // Height of the bob
    public float frequency = 1f;     // Speed of the bob

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);


    }
}
