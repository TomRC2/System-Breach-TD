using UnityEngine;

public class FarmTower : MonoBehaviour
{
    private TowerData data;
    private int currentLevel = 0;
    public static int farmCount = 0;
    [Header("Rotation")]
    public Transform rotatingPart;
    public float rotationSpeed = 90f;


    void Update()
    {
        if (rotatingPart != null)
            rotatingPart.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
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

        // feedback visual: "+$X" flotante y destello dorado sobre la farm
        Vector3 pos = transform.position;
        if (Camera.main != null) pos -= Camera.main.transform.forward * 1f;
        FXUtil.SpawnFloatingText(pos, $"+${money}", new Color(1f, 0.85f, 0.3f));
        FXUtil.SpawnImpactFlash(pos, new Color(1f, 0.85f, 0.3f, 0.5f), 0.6f, 0.25f);
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
    void OnEnable() { farmCount++; }
    // OnDisable en vez de OnDestroy: Destroy() se aplica al final del frame,
    // asi el precio de la siguiente farm se recalcula bien justo al vender.
    void OnDisable()
    {
        farmCount = Mathf.Max(0, farmCount - 1);
        if (WaveSpawner.Instance != null)
            WaveSpawner.Instance.OnWaveChanged -= OnWaveStarted;
    }
}