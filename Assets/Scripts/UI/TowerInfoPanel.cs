using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerInfoPanel : MonoBehaviour
{
    public static TowerInfoPanel Instance;

    [Header("Panel")]
    public GameObject panel;

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

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        panel.SetActive(false);
        upgradeButton.interactable = false;
        sellButton.interactable = false;

        // Refrescar botones cuando cambia el dinero
        if (EconomyManager.Instance != null)
            EconomyManager.Instance.OnMoneyChanged += _ => RefreshButtons();
    }

    public void Show(TowerController tower)
    {
        currentTower = tower;
        RefreshPanel();
        panel.SetActive(true);
    }

    public void Close()
    {
        currentTower = null;
        panel.SetActive(false);
    }

    void RefreshPanel()
    {
        TowerData data = currentTower.GetData();

        towerNameText.text = data.towerName;
        damageText.text = $"Damage: {currentTower.GetCurrentStats().damage}";
        attackSpeedText.text = $"Speed: {currentTower.GetCurrentStats().attackSpeed}";
        rangeText.text = $"Range: {currentTower.GetCurrentStats().range}";
        levelText.text = $"Level: {currentTower.GetCurrentLevel()} / {data.levels.Length}";

        // Sell
        int sellValue = Mathf.RoundToInt(data.cost * 0.5f);
        TMP_Text sellLabel = sellButton.GetComponentInChildren<TMP_Text>();
        if (sellLabel != null) sellLabel.text = $"Sell ${sellValue}";
        sellButton.interactable = true;
        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(() => SellTower(sellValue));

        RefreshButtons();
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

    void UpgradeTower()
    {
        int cost = currentTower.GetUpgradeCost();
        if (!EconomyManager.Instance.Spend(cost)) return;

        currentTower.Upgrade();
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

        Close();
    }
}