using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

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
}
