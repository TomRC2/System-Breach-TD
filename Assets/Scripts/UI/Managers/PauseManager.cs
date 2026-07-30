using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("Panel")]
    public GameObject pausePanel;

    [Header("Combat Visual Toggles")]
    public Toggle healthBarToggle;
    public Toggle damageTextToggle;

    private const string KEY_HEALTHBAR = "show_health_bar";
    private const string KEY_DAMAGETEXT = "show_damage_text";

    [Header("HUD")]
    public GameObject hud;

    [Header("Options")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle autoSkipToggle;

    private bool isPaused = false;
    public bool IsPaused => isPaused;
    private float timeScaleBeforePause = 1f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("vol_music", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("vol_sfx", 1f);
        autoSkipToggle.isOn = PlayerPrefs.GetInt("auto_skip", 0) == 1;

        healthBarToggle.isOn = PlayerPrefs.GetInt(KEY_HEALTHBAR, 1) == 1;
        damageTextToggle.isOn = PlayerPrefs.GetInt(KEY_DAMAGETEXT, 1) == 1;

        healthBarToggle.onValueChanged.AddListener(v => PlayerPrefs.SetInt(KEY_HEALTHBAR, v ? 1 : 0));
        damageTextToggle.onValueChanged.AddListener(v => PlayerPrefs.SetInt(KEY_DAMAGETEXT, v ? 1 : 0));

        musicSlider.onValueChanged.AddListener(v =>
        {
            PlayerPrefs.SetFloat("vol_music", v);
            if (AudioManager.Instance != null) AudioManager.Instance.SetMusicVolume(v);
        });

        sfxSlider.onValueChanged.AddListener(v =>
        {
            PlayerPrefs.SetFloat("vol_sfx", v);
            if (AudioManager.Instance != null) AudioManager.Instance.SetSFXVolume(v);
        });

        autoSkipToggle.onValueChanged.AddListener(v =>
            PlayerPrefs.SetInt("auto_skip", v ? 1 : 0));

        pausePanel.SetActive(false);
    }
    public void OpenTutorial()
    {
        Resume();
        PlayerPrefs.SetInt("tutorial_done", 0);
        PlayerPrefs.Save();
        GameManager.Instance.tutorialPanel.SetActive(true);
        Time.timeScale = 0f;
    }
    void Update()
    {
        // No permitir pausar cuando la partida ya termino (victoria/derrota)
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded) return;

        // Si se esta colocando una torre, Escape la cancela (no abre la pausa)
        if (PlacementManager.Instance != null &&
            (PlacementManager.Instance.HasSelection || PlacementManager.LastCancelFrame == Time.frameCount))
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        hud.SetActive(!isPaused);
        pausePanel.SetActive(isPaused);
        if (isPaused)
        {
            timeScaleBeforePause = Time.timeScale;
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = timeScaleBeforePause;
        }
    }

    public void Resume()
    {
        isPaused = false;
        hud.SetActive(true);
        pausePanel.SetActive(false);
        Time.timeScale = timeScaleBeforePause;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
    }

    public static bool IsHealthBarEnabled() => PlayerPrefs.GetInt("show_health_bar", 1) == 1;
    public static bool IsDamageTextEnabled() => PlayerPrefs.GetInt("show_damage_text", 1) == 1;
}