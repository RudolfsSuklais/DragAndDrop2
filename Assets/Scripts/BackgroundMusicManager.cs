using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusicManager : MonoBehaviour
{
    [Header("Background Music")]
    public AudioClip menuMusic;

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
        SceneManager.sceneLoaded += OnSceneLoaded;
        // Check initial scene
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void SetupAudioSource()
    {
        audioSource.loop = true;
        audioSource.volume = musicVolume;
        audioSource.playOnAwake = false;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Pārbaudām vai objekts vēl eksistē
        if (this == null || gameObject == null) return;

        if (scene.buildIndex == 0) // Menu scene
        {
            PlayMenuMusic();
        }
        else // Other scenes - stop music
        {
            StopMusic();
        }
    }

    public void PlayMenuMusic()
    {
        // Pārbaudām visas iespējamās null atsauces
        if (this == null || gameObject == null || audioSource == null)
        {
            Debug.LogError("BackgroundMusicManager: Cannot play music - null reference");
            return;
        }

        if (menuMusic != null && audioSource.clip != menuMusic)
        {
            audioSource.clip = menuMusic;
            audioSource.Play();
        }
        else if (menuMusic != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (this != null && gameObject != null && audioSource != null)
        {
            audioSource.Stop();
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (audioSource != null)
        {
            audioSource.volume = musicVolume;
        }
    }

    // Optional: Pause and Resume methods
    public void PauseMusic()
    {
        if (audioSource != null)
        {
            audioSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (audioSource != null)
        {
            audioSource.UnPause();
        }
    }
}