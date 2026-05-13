using UnityEngine;
using System;

[Serializable]
public class SoundEntry
{
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    public bool loop = false;
    public bool playOnAwake = false;

    [HideInInspector] public AudioSource source;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Sounds")]
    public SoundEntry[] sounds;

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
            return;
        }

        foreach (SoundEntry s in sounds)
        {
            if (s.clip == null)
            {
                Debug.LogWarning($"SoundManager: '{s.name}' has no AudioClip assigned.");
                continue;
            }

            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = false;

            if (s.playOnAwake)
                s.source.Play();
        }
    }

    private SoundEntry Find(string soundName)
    {
        SoundEntry s = Array.Find(sounds, x =>
            string.Equals(x.name, soundName, StringComparison.OrdinalIgnoreCase));

        if (s == null)
            Debug.LogWarning($"SoundManager: '{soundName}' not found. Check the name in the Inspector.");

        return s;
    }

    public void Play(string soundName)
    {
        SoundEntry s = Find(soundName);
        if (s?.source == null) return;

        if (s.loop)
        {
            if (!s.source.isPlaying)
                s.source.Play();
        }
        else
        {
            s.source.PlayOneShot(s.clip, s.volume);
        }
    }

    public void Stop(string soundName)
    {
        SoundEntry s = Find(soundName);
        s?.source.Stop();
    }

    public void Pause(string soundName)
    {
        Find(soundName)?.source.Pause();
    }

    public void Resume(string soundName)
    {
        Find(soundName)?.source.UnPause();
    }

    public bool IsPlaying(string soundName)
    {
        SoundEntry s = Find(soundName);
        return s?.source != null && s.source.isPlaying;
    }

    public void SetVolume(string soundName, float volume)
    {
        SoundEntry s = Find(soundName);
        if (s?.source == null) return;
        s.volume = volume;
        s.source.volume = volume;
    }

    public void SetPitch(string soundName, float pitch)
    {
        SoundEntry s = Find(soundName);
        if (s?.source == null) return;
        s.pitch = pitch;
        s.source.pitch = pitch;
    }
}