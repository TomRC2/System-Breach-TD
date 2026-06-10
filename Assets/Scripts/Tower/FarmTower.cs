using UnityEngine;

public class FarmTower : MonoBehaviour
{
    private TowerData data;
    private int currentLevel = 0;

    public void Initialize(TowerData towerData)
    {
        data = towerData;
        currentLevel = 0;
        WaveSpawner.Instance.OnWaveChanged += OnWaveStarted;
    }

    void OnWaveStarted(int current, int total)
    {
        int money = data.levels[currentLevel].moneyPerWave;
        EconomyManager.Instance.Earn(money);
    }

    public void Upgrade()
    {
        if (currentLevel >= data.levels.Length - 1) return;
        currentLevel++;
    }

    public TowerData GetData() => data;
    public TowerLevel GetCurrentLevelStats() => data.levels[currentLevel];
    public int GetCurrentLevel() => currentLevel + 1;
    public bool CanUpgrade() => currentLevel < data.levels.Length - 1;
    public int GetUpgradeCost() => data.levels[currentLevel].upgradeCost;

    void OnDestroy()
    {
        if (WaveSpawner.Instance != null)
            WaveSpawner.Instance.OnWaveChanged -= OnWaveStarted;
    }
}