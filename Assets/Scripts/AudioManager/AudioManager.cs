using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music")]
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;

    [Header("SFX")]
    public AudioClip bossSpawnSFX;
    public AudioClip placeTowerSFX;
    public AudioClip spendMoneySFX;

    private const string KEY_MUSIC = "vol_music";
    private const string KEY_SFX = "vol_sfx";

    private void Awake()
    {
        //hago un singleton para evitar problemas con multiples instancias
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadVolumes();

        }
        else
        {
            Destroy(gameObject);
        }
    }

   void LoadVolumes()
    {
        float musicVol = PlayerPrefs.GetFloat(KEY_MUSIC, 1f);
        float sfxVol = PlayerPrefs.GetFloat(KEY_SFX, 1f);

        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);   
    } 
    public void SetMusicVolume(float volume)
    {

        musicSource.volume = volume;
        PlayerPrefs.SetFloat(KEY_MUSIC, volume);
 
    }
    public void SetSFXVolume(float value)
    {
        sfxSource.volume = value;
        PlayerPrefs.SetFloat(KEY_SFX, value);
    }
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    public void PlayMenuMusic()
    {
        if (musicSource.clip == menuMusic)
            return;

        musicSource.clip = menuMusic;
        musicSource.Play();
    }

    public void PlayGameplayMusic()
    {
        if (musicSource.clip == gameplayMusic)
            return;

        musicSource.clip = gameplayMusic;
        musicSource.Play();
    }
}

