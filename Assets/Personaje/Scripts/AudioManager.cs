using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameOverMusic;

    [Header("Player SFX")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip damageSound;

    [Header("Game SFX")]
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.7f;
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;

    void Awake()
    {
        // Singleton Pattern - Solo una instancia
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeAudio()
    {
        // Si no se asignaron AudioSources, crearlos
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        // Configurar volúmenes iniciales
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }

    void Start()
    {
        // Reproducir música de fondo automáticamente
        if (backgroundMusic != null)
            PlayMusic(backgroundMusic);
    }

    #region Music Methods
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return; // Ya se está reproduciendo

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = musicVolume;
    }
    #endregion

    #region SFX Methods
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip, float volumeScale)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volumeScale);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }
    #endregion

    #region Quick Access Methods - Para usar fácilmente desde otros scripts
    public void PlayAttackSound()
    {
        PlaySFX(attackSound);
    }

    public void PlayWalkSound()
    {
        PlaySFX(walkSound, 0.5f); // Volumen más bajo para pasos
    }

    public void PlayJumpSound()
    {
        PlaySFX(jumpSound);
    }

    public void PlayDamageSound()
    {
        PlaySFX(damageSound);
    }

    public void PlayCollectSound()
    {
        PlaySFX(collectSound);
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSound);
    }

    public void PlayGameOver()
    {
        PlaySFX(gameOverSound);
        if (gameOverMusic != null)
            PlayMusic(gameOverMusic);
    }

    public void PlayBackgroundMusic()
    {
        PlayMusic(backgroundMusic);
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
    }
    #endregion

    #region Fade Effects (Bonus)
    public void FadeOutMusic(float duration)
    {
        StartCoroutine(FadeAudio(musicSource, 0f, duration));
    }

    public void FadeInMusic(float duration)
    {
        StartCoroutine(FadeAudio(musicSource, musicVolume, duration));
    }

    private System.Collections.IEnumerator FadeAudio(AudioSource source, float targetVolume, float duration)
    {
        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }

        source.volume = targetVolume;

        if (targetVolume == 0f)
            source.Stop();
    }
    #endregion
}