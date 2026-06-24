using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("Panel")]
    public GameObject pausePanel;

    [Header("HUD")]
    public GameObject hud;

    [Header("Options")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle autoSkipToggle;

    private bool isPaused = false;
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

    void Update()
    {
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
}