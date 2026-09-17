using System.Collections.Generic;
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

    [Header("UI")]
    public AudioClip buttonSound;
    private Dictionary<AudioClip, float> lastPlayedTime = new Dictionary<AudioClip, float>();
    public float minTimeBetweenSameSFX = 0.1f;
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

            // En Android/iOS, Unity limita el frame rate a 30 fps por defecto si no se
            // especifica lo contrario, mientras que en PC corre sin tope (vSyncCount = 0).
            // Eso hacia que las torres de ataque rapido (limitadas a 1 disparo por frame
            // en TowerController) tuvieran menos DPS real en celular que en PC.
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

        }
        else
        {
            Destroy(gameObject);
        }
    }

   void LoadVolumes()
    {
        float musicVol = PlayerPrefs.GetFloat(KEY_MUSIC, 0.1f);
        float sfxVol = PlayerPrefs.GetFloat(KEY_SFX, 0.1f);

        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);   
    } 
    public void SetMusicVolume(float volume)
    {

        musicSource.volume = volume;
        PlayerPrefs.SetFloat(KEY_MUSIC, volume);
 
    }
    public void PlayButtonSound()
    {
        sfxSource.PlayOneShot(buttonSound);
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
    public void PlaySFXLimited(AudioClip clip)
    {
        if (clip == null) return;

        if (lastPlayedTime.ContainsKey(clip) &&
            Time.time - lastPlayedTime[clip] < minTimeBetweenSameSFX)
            return;

        lastPlayedTime[clip] = Time.time;
        sfxSource.PlayOneShot(clip);
    }
}

