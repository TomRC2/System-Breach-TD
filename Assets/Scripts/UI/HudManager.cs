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
            waveText.text = $"Wave {current} / {total}";
    }
}