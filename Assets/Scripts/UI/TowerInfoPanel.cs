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

    [Header("Economia")]
    [Tooltip("Fraccion del dinero invertido (coste + mejoras) que se recupera al vender")]
    [Range(0f, 1f)] public float sellRefund = 0.5f;

    private TowerController currentTower;
    private BoosterTower currentBooster;
    private FarmTower currentFarm;

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
            EconomyManager.Instance.OnMoneyChanged += _ => OnMoneyChanged();
    }

    // Antes solo se refrescaba la torre de ataque: boosters y farms quedaban
    // con el boton de mejora desactivado aunque consiguieras el dinero.
    void OnMoneyChanged()
    {
        if (currentTower != null) RefreshButtons();
        else if (currentBooster != null) ShowBooster(currentBooster);
        else if (currentFarm != null) ShowFarm(currentFarm);
    }

    // Valor de venta = % de todo lo invertido (coste base + mejoras compradas)
    int SellValueFor(TowerData data, int upgradesBought)
    {
        int invested = data.cost;
        for (int i = 0; i < upgradesBought && i < data.levels.Length; i++)
            invested += data.levels[i].upgradeCost;
        return Mathf.RoundToInt(invested * sellRefund);
    }

    public void Show(TowerController tower)
    {
        PlacementManager.Instance.DeselectTower();
        TowerSelectionPanel.Instance.OnTowerPlacedOrCancelled();
        TowerSelectionPanel.Instance.panel.SetActive(false);
        currentTower = tower;
        currentBooster = null;
        currentFarm = null;
        focusContainer.SetActive(true);

        focusPrevButton.onClick.RemoveAllListeners();
        focusNextButton.onClick.RemoveAllListeners();
        focusPrevButton.onClick.AddListener(() => CycleFocus(-1));
        focusNextButton.onClick.AddListener(() => CycleFocus(1));

        RefreshPanel();
        PanelFX.Show(panel);
    }

    public void ShowBooster(BoosterTower booster)
    {
        PlacementManager.Instance.DeselectTower();
        TowerSelectionPanel.Instance.OnTowerPlacedOrCancelled();
        TowerSelectionPanel.Instance.panel.SetActive(false);
        currentBooster = booster;
        currentTower = null;
        currentFarm = null;
        focusContainer.SetActive(false);
        PanelFX.Show(panel);

        TowerData data = booster.GetData();
        TowerLevel level = booster.GetCurrentLevelStats();

        towerNameText.text = data.towerName;
        damageText.text = level.damageBonus > 0 ? $"Damage bonus: +{level.damageBonus * 100:F0}%" : "Damage bonus: -";
        attackSpeedText.text = level.attackSpeedBonus > 0 ? $"Speed bonus: +{level.attackSpeedBonus * 100:F0}%" : "Speed bonus: -";
        rangeText.text = level.rangeBonus > 0 ? $"Range bonus: +{level.rangeBonus * 100:F0}%" : "Range bonus: -";
        levelText.text = $"Level: {booster.GetCurrentLevel()} / {data.levels.Length}";

        bool canUpgrade = booster.GetCurrentLevel() < data.levels.Length;
        bool canAfford = EconomyManager.Instance.CanAfford(level.upgradeCost);

        upgradeButton.interactable = canUpgrade && canAfford;
        upgradeButton.onClick.RemoveAllListeners();
        if (canUpgrade && canAfford)
            upgradeButton.onClick.AddListener(() =>
            {
                EconomyManager.Instance.Spend(booster.GetCurrentLevelStats().upgradeCost);
                booster.Upgrade();
                booster.GetComponent<TowerScaleFX>()?.PlayUpgrade();
                TowerClickHandler handler = booster.GetComponentInChildren<TowerClickHandler>();
                if (handler != null) handler.RefreshRange();
                ShowBooster(booster);
            });

        TMP_Text upgradeLabel = upgradeButton.GetComponentInChildren<TMP_Text>();
        if (upgradeLabel != null)
            upgradeLabel.text = canUpgrade ? $"Upgrade ${level.upgradeCost}" : "Max Level";

        int sellValue = SellValueFor(data, booster.GetCurrentLevel() - 1);
        TMP_Text sellLabel = sellButton.GetComponentInChildren<TMP_Text>();
        if (sellLabel != null) sellLabel.text = $"Sell ${sellValue}";
        sellButton.interactable = true;
        sellButton.onClick.RemoveAllListeners();
        BoosterTower capturedBooster = booster;
        sellButton.onClick.AddListener(() => Sell(capturedBooster.gameObject, sellValue));
    }

    public void ShowFarm(FarmTower farm)
    {
        currentTower = null;
        currentBooster = null;
        currentFarm = farm;
        focusContainer.SetActive(false);
        TowerSelectionPanel.Instance.panel.SetActive(false);
        PlacementManager.Instance.DeselectTower();
        TowerSelectionPanel.Instance.OnTowerPlacedOrCancelled();
        PanelFX.Show(panel);

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
                capturedFarm.GetComponent<TowerScaleFX>()?.PlayUpgrade();
                ShowFarm(capturedFarm);
            });

        TMP_Text upgradeLabel = upgradeButton.GetComponentInChildren<TMP_Text>();
        if (upgradeLabel != null)
            upgradeLabel.text = canUpgrade ? $"Upgrade ${farm.GetUpgradeCost()}" : "Max Level";

        int sellValue = SellValueFor(data, farm.GetCurrentLevel() - 1);
        TMP_Text sellLabel = sellButton.GetComponentInChildren<TMP_Text>();
        if (sellLabel != null) sellLabel.text = $"Sell ${sellValue}";
        sellButton.interactable = true;
        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(() => Sell(capturedFarm.gameObject, sellValue));
    }

    public void Close()
    {
        currentTower = null;
        currentBooster = null;
        currentFarm = null;
        focusContainer.SetActive(true);
        panel.SetActive(false);
    }

    void RefreshPanel()
    {
        TowerData data = currentTower.GetData();
        TowerLevel base_ = currentTower.GetCurrentStats();
        TowerLevel boost = currentTower.GetActiveBoost();

        towerNameText.text = data.towerName;

        string dmg = boost != null && boost.damageBonus > 0
            ? $"Damage: {base_.damage:F0} (+{base_.damage * boost.damageBonus:F0})"
            : $"Damage: {base_.damage:F0}";
        if (base_.critChance > 0f)
            dmg += $"  |  Crit: {base_.critChance * 100:F0}%";
        damageText.text = dmg;

        attackSpeedText.text = boost != null && boost.attackSpeedBonus > 0
            ? $"Speed: {base_.attackSpeed:F1} (+{base_.attackSpeed * boost.attackSpeedBonus:F1})"
            : $"Speed: {base_.attackSpeed:F1}";

        string rng = boost != null && boost.rangeBonus > 0
            ? $"Range: {base_.range:F1} (+{base_.range * boost.rangeBonus:F1})"
            : $"Range: {base_.range:F1}";
        if (base_.splashRadius > 0f) rng += $"  |  Splash: {base_.splashRadius:F1}";
        if (base_.slowAmount > 0f) rng += $"  |  Slow: {base_.slowAmount * 100:F0}%";
        rangeText.text = rng;

        levelText.text = $"Level: {currentTower.GetCurrentLevel()} / {data.levels.Length}";

        int sellValue = SellValueFor(data, currentTower.GetCurrentLevel() - 1);
        TMP_Text sellLabel = sellButton.GetComponentInChildren<TMP_Text>();
        if (sellLabel != null) sellLabel.text = $"Sell ${sellValue}";
        sellButton.interactable = true;
        sellButton.onClick.RemoveAllListeners();
        TowerController captured = currentTower;
        sellButton.onClick.AddListener(() => Sell(captured.gameObject, sellValue));

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
        currentTower.GetComponent<TowerScaleFX>()?.PlayUpgrade();
        TowerClickHandler handler = currentTower.GetComponentInChildren<TowerClickHandler>();
        if (handler != null) handler.RefreshRange();

        if (!currentTower.CanUpgrade())
            AchievementManager.Instance?.RegisterMaxOut();

        RefreshPanel();
    }

    // Venta unificada para los tres tipos de torre (antes habia 3 metodos casi identicos)
    void Sell(GameObject towerRoot, int sellValue)
    {
        EconomyManager.Instance.Earn(sellValue);
        AchievementManager.Instance?.RegisterSell();

        TowerClickHandler handler = towerRoot.GetComponentInChildren<TowerClickHandler>();
        if (handler != null) handler.Deselect(); // Deselect ya llama Close() internamente
        else Close();

        GridCell[] cells = FindObjectsByType<GridCell>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (GridCell cell in cells)
        {
            if (cell.IsOccupiedBy(towerRoot))
            {
                cell.FreeCellAndDestroy();
                break;
            }
        }

        // La celda vuelve a estar libre: refrescar precios (las farms cambian de coste)
        TowerSelectionPanel.Instance?.RefreshAllLabels();
    }
}
