using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerSelectionPanel : MonoBehaviour
{
    public static TowerSelectionPanel Instance;

    [Header("Panel")]
    public GameObject panel;

    [Header("Torres disponibles")]
    public TowerData[] availableTowers;

    [Header("Prefab de botón")]
    public GameObject towerButtonPrefab;
    public Transform buttonContainer;

    [Header("Grid")]
    public GridCell[] allCells;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        panel.SetActive(false);
        GenerateButtons();
        HideGrid();

        if (EconomyManager.Instance != null)
            EconomyManager.Instance.OnMoneyChanged += _ => RefreshButtons();
    }

    void GenerateButtons()
    {
        foreach (TowerData data in availableTowers)
        {
            GameObject btn = Instantiate(towerButtonPrefab, buttonContainer);

            TMP_Text label = btn.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = $"{data.towerName}\n${data.cost}";

            TowerData captured = data;
            Button button = btn.GetComponent<Button>();
            button.onClick.AddListener(() => SelectTower(captured));
        }
    }

    void RefreshButtons()
    {
        Button[] buttons = buttonContainer.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length && i < availableTowers.Length; i++)
        {
            buttons[i].interactable =
                EconomyManager.Instance.CanAfford(availableTowers[i].cost);
        }
    }

    public void TogglePanel()
    {
        bool isOpen = !panel.activeSelf;
        panel.SetActive(isOpen);

        if (!isOpen)
        {
            PlacementManager.Instance.DeselectTower();
            HideGrid();
        }
    }

    void SelectTower(TowerData data)
    {
        PlacementManager.Instance.SelectTower(data);
        panel.SetActive(false);
        ShowGrid();
    }

    public void OnTowerPlacedOrCancelled()
    {
        HideGrid();
    }

    public void ShowGrid()
    {
        foreach (GridCell cell in allCells)
            if (!cell.isOccupied) cell.gameObject.SetActive(true);
    }

    public void HideGrid()
    {
        foreach (GridCell cell in allCells)
            cell.gameObject.SetActive(false);
    }
}