using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    [Header("Background Music")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;

    private AudioSource audioSource;
    private static BackgroundMusicManager instance;

    void Awake()
    {
        // Implement singleton pattern to persist across scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        SetupAudioSource();
    }

    void Start()
    {
        // Play menu music when the game starts
        PlayMenuMusic();
    }

    void SetupAudioSource()
    {
        audioSource.loop = true;
        audioSource.volume = musicVolume;
        audioSource.playOnAwake = false;
    }

    public void PlayMenuMusic()
    {
        if (menuMusic != null && audioSource.clip != menuMusic)
        {
            audioSource.clip = menuMusic;
            audioSource.Play();
        }
    }

    public void PlayGameMusic()
    {
        if (gameMusic != null && audioSource.clip != gameMusic)
        {
            audioSource.clip = gameMusic;
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        audioSource.volume = musicVolume;
    }

    // Optional: Pause and Resume methods
    public void PauseMusic()
    {
        audioSource.Pause();
    }

    public void ResumeMusic()
    {
        audioSource.UnPause();
    }
}