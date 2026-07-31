using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Level")]
    public int levelNumber;
    public float levelTimer = 0f;
    private bool levelStarted = false;
    [Header("Panels")]
    public GameObject victoryPanel;
    public GameObject gameOverPanel;
    [Header("Tutorial")]
    public GameObject tutorialPanel;
    [Header("HUD")]
    public GameObject hud;
    [Header("References")]
    public ComputerHealth computerHealth;
    [Header("Score UI")]
    public TMP_Text victoryScoreText;
    public TMP_Text victoryHighscoreText;
    public TMP_Text gameOverScoreText;

    private bool gameEnded = false;
    public bool IsGameEnded => gameEnded;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        victoryPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        if (tutorialPanel != null)
            tutorialPanel.SetActive(!TutorialManager.IsTutorialDone());
    }
    void Update()
    {
        if (levelStarted)
            levelTimer += Time.unscaledDeltaTime;
    }

    public void StartLevelTimer()
    {
        levelStarted = true;
        levelTimer = 0f;
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
        if (EconomyManager.Instance.GetTotalSpent() <= 750)
            AchievementManager.Instance?.RegisterFrugal();
        if (levelTimer <= 120f)
            AchievementManager.Instance?.RegisterSpeedRun();
        AchievementManager.Instance?.RegisterLevelCompleted();
        if (computerHealth != null && computerHealth.currentHP >= computerHealth.maxHP)
            AchievementManager.Instance?.RegisterNoDamage();
        PanelFX.Show(victoryPanel);
    }

    public void GameOver()
    {
        if (gameEnded) return;
        gameEnded = true;

        hud.SetActive(false);
        Time.timeScale = 0f;

        if (ScoreManager.Instance != null && gameOverScoreText != null)
            gameOverScoreText.text = $"Puntaje: {ScoreManager.Instance.GetScore()}";

        PanelFX.Show(gameOverPanel);
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