using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Setup")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Settings")]
    [Range(0f, 1f)]public float sfxVolume = 1f;
    [Range(0f, 1f)]public float musicVolume = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else Instance = this;
    }

    void Start()
    {
        if (sfxSource == null)
        {
            Debug.LogWarning("SFX Source at " + this.gameObject + " has not been set!");
        }
        if (musicSource == null)
        {
            Debug.LogWarning("Music Source at " + this.gameObject + " has not been set!");
        }
    }

    public void PlaySfx(AudioClip audio)
    {
        sfxSource.PlayOneShot(audio, sfxVolume);
    }

    public void PreloadMusic(AudioClip audio, bool loop = false)
    {
        musicSource.Stop();
        musicSource.loop = loop;
        musicSource.clip = audio;
    }

    public void PlayPreloadedMusic()
    {
        musicSource.Play();
    }

    public void PlayMusic(AudioClip audio)
    {
        musicSource.Stop();
        musicSource.clip = audio;
        musicSource.Play();
    }

    public void UpdateAudio()
    {
        sfxSource.volume = sfxVolume;
        musicSource.volume = musicVolume;
    }
}