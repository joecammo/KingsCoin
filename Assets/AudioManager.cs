using UnityEngine;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    [Header("Audio Clips")]
    public AudioClip jumpSound;
    public AudioClip gameOverSound;

    [Header("Sound Settings")]
    public float volume = 1f;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayJumpSound()
    {
        if (jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound, volume);
        }
    }

    public void PlayGameOverSound()
    {
        if (gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound, volume);
        }
    }
}
