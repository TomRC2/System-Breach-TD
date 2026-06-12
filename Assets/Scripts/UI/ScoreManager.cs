using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public event Action<int> OnScoreChanged;

    private int currentScore = 0;

    // Multiplicadores de puntaje
    private const float KILL_MULTIPLIER = 10f;
    private const float BOSS_MULTIPLIER = 50f;
    private const string KEY_HIGHSCORE = "highscore_level_";

    void Awake()
    {
        Instance = this;
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        OnScoreChanged?.Invoke(currentScore);
    }

    public void RegisterKill(float reward, bool isBoss = false)
    {
        float multiplier = isBoss ? BOSS_MULTIPLIER : KILL_MULTIPLIER;
        int points = Mathf.RoundToInt(reward * multiplier);
        AddScore(points);
    }

    public int GetScore() => currentScore;

    // Guarda el highscore del nivel si es mayor al anterior
    public int SaveAndGetHighscore(int levelNumber)
    {
        string key = KEY_HIGHSCORE + levelNumber;
        int previous = PlayerPrefs.GetInt(key, 0);
        if (currentScore > previous)
        {
            PlayerPrefs.SetInt(key, currentScore);
            PlayerPrefs.Save();
        }
        return Mathf.Max(currentScore, previous);
    }

    public static int GetHighscore(int levelNumber)
    {
        return PlayerPrefs.GetInt(KEY_HIGHSCORE + levelNumber, 0);
    }
}