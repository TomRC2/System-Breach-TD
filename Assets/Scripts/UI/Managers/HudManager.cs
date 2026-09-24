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

    private int lastWaveCurrent = 1, lastWaveTotal = 1;
    private int lastMoney = 0;
    private int lastScore = 0;

    void Start()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged += RefreshTexts;

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
        lastWaveCurrent = current;
        lastWaveTotal = total;
        if (waveText != null)
            waveText.text = string.Format(LocalizationManager.Tr("Wave {0}/{1}"), current, total);
    }

    void UpdateMoney(int amount)
    {
        lastMoney = amount;
        if (moneyText != null)
        {
            moneyText.text = string.Format(LocalizationManager.Tr("RAM: ${0}"), amount);
            Punch(moneyText.transform);
        }
    }

    void UpdateScore(int score)
    {
        lastScore = score;
        if (scoreText != null)
        {
            scoreText.text = string.Format(LocalizationManager.Tr("Score: {0}"), score);
            Punch(scoreText.transform);
        }
    }

    // Re-formatea los textos ya mostrados si el idioma cambia en pleno gameplay
    void RefreshTexts()
    {
        if (waveText != null) waveText.text = string.Format(LocalizationManager.Tr("Wave {0}/{1}"), lastWaveCurrent, lastWaveTotal);
        if (moneyText != null) moneyText.text = string.Format(LocalizationManager.Tr("RAM: ${0}"), lastMoney);
        if (scoreText != null) scoreText.text = string.Format(LocalizationManager.Tr("Score: {0}"), lastScore);
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