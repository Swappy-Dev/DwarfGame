using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("SFX")]
    public AudioClip miningHit;
    public AudioClip miningBreak;
    public AudioClip pickupCoin;
    public AudioClip hitHurt;
    public AudioClip death;
    public AudioClip swoosh;
    public AudioClip footstep;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        audioSource.PlayOneShot(clip, volume);
    }
}