using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Combat Visual Toggles")]
    public Toggle healthBarToggle;
    public Toggle damageTextToggle;

    private const string KEY_HEALTHBAR = "show_health_bar";
    private const string KEY_DAMAGETEXT = "show_damage_text";

    [Header("Toggle")]
    public Toggle autoSkipToggle;

    private const string KEY_MUSIC = "vol_music";
    private const string KEY_SFX = "vol_sfx";
    private const string KEY_AUTOSKIP = "auto_skip";

    void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat(KEY_MUSIC, 1f);
        sfxSlider.value = PlayerPrefs.GetFloat(KEY_SFX, 1f);
        autoSkipToggle.isOn = PlayerPrefs.GetInt(KEY_AUTOSKIP, 0) == 1;

        healthBarToggle.isOn = PlayerPrefs.GetInt(KEY_HEALTHBAR, 1) == 1;
        damageTextToggle.isOn = PlayerPrefs.GetInt(KEY_DAMAGETEXT, 1) == 1;

        healthBarToggle.onValueChanged.AddListener(v => PlayerPrefs.SetInt(KEY_HEALTHBAR, v ? 1 : 0));
        damageTextToggle.onValueChanged.AddListener(v => PlayerPrefs.SetInt(KEY_DAMAGETEXT, v ? 1 : 0));

        ApplyMusic(musicSlider.value);
        ApplySFX(sfxSlider.value);

        musicSlider.onValueChanged.AddListener(ApplyMusic);
        sfxSlider.onValueChanged.AddListener(ApplySFX);
        autoSkipToggle.onValueChanged.AddListener(ApplyAutoSkip);
    }

    void ApplyMusic(float value)
    {
        PlayerPrefs.SetFloat(KEY_MUSIC, value);
        if (AudioManager.Instance != null) AudioManager.Instance.SetMusicVolume(value);
    }

    void ApplySFX(float value)
    {
        PlayerPrefs.SetFloat(KEY_SFX, value);
        if (AudioManager.Instance != null) AudioManager.Instance.SetSFXVolume(value);
    }

    void ApplyAutoSkip(bool value)
    {
        PlayerPrefs.SetInt(KEY_AUTOSKIP, value ? 1 : 0);
    }

    public void Back()
    {
        PlayerPrefs.Save();
        MenuManager.Instance.GoHome();
    }

    public static bool IsAutoSkipEnabled()
    {
        return PlayerPrefs.GetInt(KEY_AUTOSKIP, 0) == 1;
    }

    public static bool IsHealthBarEnabled() => PlayerPrefs.GetInt("show_health_bar", 1) == 1;
    public static bool IsDamageTextEnabled() => PlayerPrefs.GetInt("show_damage_text", 1) == 1;
}