using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerInfoPanel : MonoBehaviour
{
    public static TowerInfoPanel Instance;

    [Header("Panel")]
    public GameObject panel;

    [Header("Focus")]
    public TMP_Text focusModeText;
    public Button focusPrevButton;
    public Button focusNextButton;
    public GameObject focusContainer;

    [Header("Texts")]
    public TMP_Text towerNameText;
    public TMP_Text damageText;
    public TMP_Text attackSpeedText;
    public TMP_Text rangeText;
    public TMP_Text levelText;

    [Header("Buttons")]
    public Button upgradeButton;
    public Button sellButton;

    private TowerController currentTower;
    private BoosterTower currentBooster;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        panel.SetActive(false);
        upgradeButton.interactable = false;
        sellButton.interactable = false;

        if (EconomyManager.Instance != null)
            EconomyManager.Instance.OnMoneyChanged += _ => RefreshButtons();
    }

    public void Show(TowerController tower)
    {
        PlacementManager.Instance.DeselectTower();
        TowerSelectionPanel.Instance.OnTowerPlacedOrCancelled();
        TowerSelectionPanel.Instance.panel.SetActive(false);
        currentTower = tower;
        currentBooster = null;
        focusContainer.SetActive(true);

        focusPrevButton.onClick.RemoveAllListeners();
        focusNextButton.onClick.RemoveAllListeners();
        focusPrevButton.onClick.AddListener(() => CycleFocus(-1));
        focusNextButton.onClick.AddListener(() => CycleFocus(1));

        RefreshPanel();
        panel.SetActive(true);
    }

    public void ShowBooster(BoosterTower booster)
    {
        PlacementManager.Instance.DeselectTower();
        TowerSelectionPanel.Instance.OnTowerPlacedOrCancelled();
        TowerSelectionPanel.Instance.panel.SetActive(false);
        currentBooster = booster;
        currentTower = null;
        focusContainer.SetActive(false);
        panel.SetActive(true);

        TowerData data = booster.GetData();
        TowerLevel level = booster.GetCurrentLevelStats();

        towerNameText.text = data.towerName;
        damageText.text = level.damageBonus > 0 ? $"Damage bonus: +{level.damageBonus * 100:F0}%" : "Damage bonus: -";
        attackSpeedText.text = level.attackSpeedBonus > 0 ? $"Speed bonus: +{level.attackSpeedBonus * 100:F0}%" : "Speed bonus: -";
        rangeText.text = level.rangeBonus > 0 ? $"Range bonus: +{level.rangeBonus * 100:F0}%" : "Range bonus: -";
        levelText.text = $"Level: {booster.GetCurrentLevel()} / {data.levels.Length}";

        bool canUpgrade = booster.GetCurrentLevel() < booster.GetData().levels.Length;
        bool canAfford = EconomyManager.Instance.CanAfford(booster.GetCurrentLevelStats().upgradeCost);

        upgradeButton.interactable = canUpgrade && canAfford;
        upgradeButton.onClick.RemoveAllListeners();
        if (canUpgrade && canAfford)
            upgradeButton.onClick.AddListener(() =>
            {
                EconomyManager.Instance.Spend(booster.GetCurrentLevelStats().upgradeCost);
                booster.Upgrade();
                booster.GetComponent<TowerClickHandler>().RefreshRange();
                ShowBooster(booster);
            });

        TMP_Text upgradeLabel = upgradeButton.GetComponentInChildren<TMP_Text>();
        if (upgradeLabel != null)
            upgradeLabel.text = canUpgrade ? $"Upgrade ${booster.GetCurrentLevelStats().upgradeCost}" : "Max Level";

        int sellValue = Mathf.RoundToInt(data.cost * 0.5f);
        TMP_Text sellLabel = sellButton.GetComponentInChildren<TMP_Text>();
        if (sellLabel != null) sellLabel.text = $"Sell ${sellValue}";
        sellButton.interactable = true;
        sellButton.onClick.RemoveAllListeners();
        BoosterTower capturedBooster = booster;
        sellButton.onClick.AddListener(() => SellBooster(capturedBooster, sellValue));
    }
    public void ShowFarm(FarmTower farm)
    {
        currentTower = null;
        currentBooster = null;
        focusContainer.SetActive(false);
        TowerSelectionPanel.Instance.panel.SetActive(false);
        PlacementManager.Instance.DeselectTower();
        TowerSelectionPanel.Instance.OnTowerPlacedOrCancelled();
        panel.SetActive(true);

        TowerData data = farm.GetData();
        TowerLevel level = farm.GetCurrentLevelStats();

        towerNameText.text = data.towerName;
        damageText.text = $"Money/Wave: ${level.moneyPerWave}";
        attackSpeedText.text = "-";
        rangeText.text = "-";
        levelText.text = $"Level: {farm.GetCurrentLevel()} / {data.levels.Length}";

        bool canUpgrade = farm.CanUpgrade();
        bool canAfford = EconomyManager.Instance.CanAfford(farm.GetUpgradeCost());

        upgradeButton.interactable = canUpgrade && canAfford;
        upgradeButton.onClick.RemoveAllListeners();
        FarmTower capturedFarm = farm;
        if (canUpgrade && canAfford)
            upgradeButton.onClick.AddListener(() =>
            {
                EconomyManager.Instance.Spend(capturedFarm.GetUpgradeCost());
                capturedFarm.Upgrade();
                ShowFarm(capturedFarm);
            });

        TMP_Text upgradeLabel = upgradeButton.GetComponentInChildren<TMP_Text>();
        if (upgradeLabel != null)
            upgradeLabel.text = canUpgrade ? $"Upgrade ${farm.GetUpgradeCost()}" : "Max Level";

        int sellValue = Mathf.RoundToInt(data.cost * 0.5f);
        TMP_Text sellLabel = sellButton.GetComponentInChildren<TMP_Text>();
        if (sellLabel != null) sellLabel.text = $"Sell ${sellValue}";
        sellButton.interactable = true;
        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(() => SellFarm(capturedFarm, sellValue));
    }

    void SellFarm(FarmTower farm, int sellValue)
    {
        farm.GetComponent<TowerClickHandler>().Deselect();
        EconomyManager.Instance.Earn(sellValue);

        GameObject farmRoot = farm.gameObject;
        GridCell[] cells = FindObjectsByType<GridCell>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (GridCell cell in cells)
        {
            if (cell.IsOccupiedBy(farmRoot))
            {
                cell.FreeCellAndDestroy();
                break;
            }
        }

        Close();
    }

    public void Close()
    {
        currentTower = null;
        currentBooster = null;
        focusContainer.SetActive(true);
        panel.SetActive(false);
    }

    void RefreshPanel()
    {
        TowerData data = currentTower.GetData();
        TowerLevel base_ = currentTower.GetCurrentStats();
        TowerLevel boost = currentTower.GetActiveBoost();

        towerNameText.text = data.towerName;

        damageText.text = boost != null && boost.damageBonus > 0
            ? $"Damage: {base_.damage:F0} (+{base_.damage * boost.damageBonus:F0})"
            : $"Damage: {base_.damage:F0}";

        attackSpeedText.text = boost != null && boost.attackSpeedBonus > 0
            ? $"Speed: {base_.attackSpeed:F1} (+{base_.attackSpeed * boost.attackSpeedBonus:F1})"
            : $"Speed: {base_.attackSpeed:F1}";

        rangeText.text = boost != null && boost.rangeBonus > 0
            ? $"Range: {base_.range:F1} (+{base_.range * boost.rangeBonus:F1})"
            : $"Range: {base_.range:F1}";

        levelText.text = $"Level: {currentTower.GetCurrentLevel()} / {data.levels.Length}";

        int sellValue = Mathf.RoundToInt(data.cost * 0.5f);
        TMP_Text sellLabel = sellButton.GetComponentInChildren<TMP_Text>();
        if (sellLabel != null) sellLabel.text = $"Sell ${sellValue}";
        sellButton.interactable = true;
        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(() => SellTower(sellValue));

        RefreshButtons();
        RefreshFocus();
    }

    void RefreshButtons()
    {
        if (currentTower == null) return;

        bool canUpgrade = currentTower.CanUpgrade();
        int upgradeCost = currentTower.GetUpgradeCost();
        bool canAfford = EconomyManager.Instance.CanAfford(upgradeCost);

        TMP_Text upgradeLabel = upgradeButton.GetComponentInChildren<TMP_Text>();
        if (upgradeLabel != null)
            upgradeLabel.text = canUpgrade ? $"Upgrade ${upgradeCost}" : "Max Level";

        upgradeButton.interactable = canUpgrade && canAfford;
        upgradeButton.onClick.RemoveAllListeners();
        if (canUpgrade && canAfford)
            upgradeButton.onClick.AddListener(() => UpgradeTower());
    }

    void CycleFocus(int direction)
    {
        int total = System.Enum.GetValues(typeof(FocusMode)).Length;
        int current = (int)currentTower.focusMode;
        currentTower.focusMode = (FocusMode)((current + direction + total) % total);
        focusModeText.text = currentTower.focusMode.ToString();
    }

    void RefreshFocus()
    {
        focusModeText.text = currentTower.focusMode.ToString();
    }

    void UpgradeTower()
    {
        int cost = currentTower.GetUpgradeCost();
        if (!EconomyManager.Instance.Spend(cost)) return;

        currentTower.Upgrade();
        currentTower.GetComponent<TowerClickHandler>().RefreshRange();
        RefreshPanel();
    }

    void SellTower(int sellValue)
    {
        EconomyManager.Instance.Earn(sellValue);

        GameObject towerRoot = currentTower.gameObject;

        GridCell[] cells = FindObjectsByType<GridCell>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (GridCell cell in cells)
        {
            if (cell.IsOccupiedBy(towerRoot))
            {
                cell.FreeCellAndDestroy();
                break;
            }
        }

        currentTower.GetComponent<TowerClickHandler>().Deselect();
        Close();
    }

    void SellBooster(BoosterTower booster, int sellValue)
    {
        booster.GetComponent<TowerClickHandler>().Deselect();
        EconomyManager.Instance.Earn(sellValue);

        GameObject boosterRoot = booster.gameObject;

        GridCell[] cells = FindObjectsByType<GridCell>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (GridCell cell in cells)
        {
            if (cell.IsOccupiedBy(boosterRoot))
            {
                cell.FreeCellAndDestroy();
                break;
            }
        }

        Close();
    }
}