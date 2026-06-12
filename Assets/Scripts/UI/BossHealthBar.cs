using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Asignar en el Inspector al objeto de la barra de boss en el HUD.
// Empieza oculto. Se activa automaticamente cuando spawnea el boss.
public class BossHealthBar : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public Slider hpSlider;
    public TMP_Text hpText;
    public TMP_Text bossNameText;

    private EnemyHealth trackedBoss;

    void Start()
    {
        panel.SetActive(false);

        if (WaveSpawner.Instance != null)
            WaveSpawner.Instance.OnBossSpawned += OnBossSpawned;
    }

    void OnDestroy()
    {
        if (WaveSpawner.Instance != null)
            WaveSpawner.Instance.OnBossSpawned -= OnBossSpawned;
    }

    void OnBossSpawned(EnemyHealth boss)
    {
        trackedBoss = boss;


        UpdateBar(boss.maxHP, boss.maxHP);
        panel.SetActive(true);

        boss.OnDeath += HideBar;
        boss.OnReach += HideBar;
    }

    void Update()
    {
        if (trackedBoss == null) return;
        UpdateBar(trackedBoss.GetCurrentHP(), trackedBoss.maxHP);
    }

    void UpdateBar(float current, float max)
    {
        if (hpSlider != null)
            hpSlider.value = current / max;
        if (hpText != null)
            hpText.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
    }

    void HideBar()
    {
        trackedBoss = null;
        panel.SetActive(false);
    }
}
