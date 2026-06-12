using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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

    [Header("Score UI")]
    public TMP_Text victoryScoreText;
    public TMP_Text victoryHighscoreText;
    public TMP_Text gameOverScoreText;

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
        if (gameEnded) return;
        gameEnded = true;

        hud.SetActive(false);
        Time.timeScale = 0f;

        LevelSelectManager.UnlockNextLevel(levelNumber);

        if (ScoreManager.Instance != null)
        {
            int score = ScoreManager.Instance.GetScore();
            int highscore = ScoreManager.Instance.SaveAndGetHighscore(levelNumber);

            if (victoryScoreText != null)
                victoryScoreText.text = $"Puntaje: {score}";

            if (victoryHighscoreText != null)
                victoryHighscoreText.text = score >= highscore
                    ? "¡Nuevo récord!"
                    : $"Récord: {highscore}";
        }

        victoryPanel.SetActive(true);
    }

    public void GameOver()
    {
        if (gameEnded) return;
        gameEnded = true;

        hud.SetActive(false);
        Time.timeScale = 0f;

        if (ScoreManager.Instance != null && gameOverScoreText != null)
            gameOverScoreText.text = $"Puntaje: {ScoreManager.Instance.GetScore()}";

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