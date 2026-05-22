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
    }

    public void Show(TowerController tower)
    {
        currentTower = tower;
        TowerData data = tower.GetData();

        towerNameText.text = data.towerName;
        damageText.text = $"Damage: {data.damage}";
        attackSpeedText.text = $"Speed: {data.attackSpeed}";
        rangeText.text = $"Range: {data.range}";
        levelText.text = $"Level: 1";

        panel.SetActive(true);
    }

    public void Close()
    {
        currentTower = null;
        panel.SetActive(false);
    }
}