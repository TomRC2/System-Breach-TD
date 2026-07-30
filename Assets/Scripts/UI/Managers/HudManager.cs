using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Computer HP")]
    public TMP_Text hpText;
    public Slider hpSlider;

    [Header("Wave")]
    public TMP_Text waveText;

    [Header("Money")]
    public TMP_Text moneyText;

    [Header("Score")]
    public TMP_Text scoreText;

    void Start()
    {
        ComputerHealth computer = FindFirstObjectByType<ComputerHealth>();
        if (computer != null)
        {
            computer.OnHPChanged += UpdateHP;
            UpdateHP(computer.maxHP, computer.maxHP);
        }

        WaveSpawner spawner = FindFirstObjectByType<WaveSpawner>();
        if (spawner != null)
        {
            spawner.OnWaveChanged += UpdateWave;
            UpdateWave(1, spawner.waves.Count);
        }

        if (EconomyManager.Instance != null)
        {
            EconomyManager.Instance.OnMoneyChanged += UpdateMoney;
            UpdateMoney(EconomyManager.Instance.GetMoney());
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += UpdateScore;
            UpdateScore(0);
        }
    }

    void UpdateHP(float current, float max)
    {
        if (hpText != null)
            hpText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
        if (hpSlider != null)
            hpSlider.value = current / max;
    }

    void UpdateWave(int current, int total)
    {
        if (waveText != null)
            waveText.text = $"Wave {current}/{total}";
    }

    void UpdateMoney(int amount)
    {
        if (moneyText != null)
        {
            moneyText.text = $"RAM: ${amount}";
            Punch(moneyText.transform);
        }
    }

    void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
            Punch(scoreText.transform);
        }
    }

    // Pequenio efecto de escala al cambiar un valor del HUD
    void Punch(Transform t)
    {
        if (!isActiveAndEnabled) return;
        StartCoroutine(PunchRoutine(t));
    }

    System.Collections.IEnumerator PunchRoutine(Transform t)
    {
        float duration = 0.15f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float k = 1f + 0.2f * Mathf.Sin(Mathf.Clamp01(elapsed / duration) * Mathf.PI);
            t.localScale = Vector3.one * k;
            yield return null;
        }
        t.localScale = Vector3.one;
    }
}