using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Level")]
    public int levelNumber;

    [Header("Panels")]
    public GameObject victoryPanel;
    public GameObject gameOverPanel;

    [Header("HUD")]
    public GameObject hud;

    private bool gameEnded = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        victoryPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    public void Victory()
    {
        hud.SetActive(false);
        if (gameEnded) return;
        gameEnded = true;

        LevelSelectManager.UnlockNextLevel(levelNumber);
        Time.timeScale = 0f;
        victoryPanel.SetActive(true);
    }

    public void GameOver()
    {
        hud.SetActive(false);
        if (gameEnded) return;
        gameEnded = true;

        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level" + (levelNumber + 1));
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level" + levelNumber);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}