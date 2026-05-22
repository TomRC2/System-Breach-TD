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

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat("vol_music", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("vol_sfx", 1f);
        autoSkipToggle.isOn = PlayerPrefs.GetInt("auto_skip", 0) == 1;

        musicSlider.onValueChanged.AddListener(v => PlayerPrefs.SetFloat("vol_music", v));
        sfxSlider.onValueChanged.AddListener(v => PlayerPrefs.SetFloat("vol_sfx", v));
        autoSkipToggle.onValueChanged.AddListener(v => PlayerPrefs.SetInt("auto_skip", v ? 1 : 0));

        pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        hud.SetActive(false);
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void Resume()
    {
        hud.SetActive(true);
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
    }
}
