using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Toggle")]
    public Toggle autoSkipToggle;

    // PlayerPrefs keys
    private const string KEY_MUSIC = "vol_music";
    private const string KEY_SFX = "vol_sfx";
    private const string KEY_AUTOSKIP = "auto_skip";

    void Start()
    {
        // Cargar valores guardados (defaults: música 1, sfx 1, autoskip off)
        musicSlider.value = PlayerPrefs.GetFloat(KEY_MUSIC, 1f);
        sfxSlider.value = PlayerPrefs.GetFloat(KEY_SFX, 1f);
        autoSkipToggle.isOn = PlayerPrefs.GetInt(KEY_AUTOSKIP, 0) == 1;

        // Aplicar al arrancar
        ApplyMusic(musicSlider.value);
        ApplySFX(sfxSlider.value);

        // Listeners
        musicSlider.onValueChanged.AddListener(ApplyMusic);
        sfxSlider.onValueChanged.AddListener(ApplySFX);
        autoSkipToggle.onValueChanged.AddListener(ApplyAutoSkip);
    }

    void ApplyMusic(float value)
    {
        // TODO: conectar con AudioManager cuando esté listo (Lolo)
        AudioManager.Instance.SetMusicVolume(value);
    }

    void ApplySFX(float value)
    {
        // TODO: conectar con AudioManager cuando esté listo (Lolo)
        AudioManager.Instance.SetSFXVolume(value);
        // PlayerPrefs.SetFloat(KEY_SFX, value);
    }

    void ApplyAutoSkip(bool value)
    {
        PlayerPrefs.SetInt(KEY_AUTOSKIP, value ? 1 : 0);
    }

    // Llamado por botón "Volver" en el panel de opciones
    public void Back()
    {
        PlayerPrefs.Save();
        MenuManager.Instance.GoHome();
    }

    // Acceso global para que WaveSpawner consulte si Auto Skip está activo
    public static bool IsAutoSkipEnabled()
    {
        return PlayerPrefs.GetInt(KEY_AUTOSKIP, 0) == 1;
    }
}
