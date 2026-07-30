using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TowerSelectionPanel : MonoBehaviour
{
    public static TowerSelectionPanel Instance;

    [Header("Panel")]
    public GameObject panel;

    [Header("Torres disponibles")]
    public TowerData[] availableTowers;

    [Header("Prefab de bot�n")]
    public GameObject towerButtonPrefab;
    public Transform buttonContainer;

    [Header("Grid")]
    public GridCell[] allCells;

    private Dictionary<TowerData, (TMP_Text label, Button button)> buttonLabels =
        new Dictionary<TowerData, (TMP_Text, Button)>();

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
            EconomyManager.Instance.OnMoneyChanged += _ => RefreshAllLabels();
    }

    void GenerateButtons()
    {
        foreach (TowerData data in availableTowers)
        {
            GameObject btn = Instantiate(towerButtonPrefab, buttonContainer);

            TMP_Text label = btn.GetComponentInChildren<TMP_Text>();
            Button button = btn.GetComponent<Button>();

            if (label != null && button != null)
            {
                buttonLabels[data] = (label, button);
                RefreshButtonLabel(data);
            }

            TowerData captured = data;
            button.onClick.AddListener(() => SelectTower(captured));
        }
    }

    public void RefreshButtonLabel(TowerData data)
    {
        if (!buttonLabels.ContainsKey(data)) return;
        int cost = data.GetEffectiveCost();
        buttonLabels[data].label.text = $"{data.towerName}\n${cost}";
        buttonLabels[data].button.interactable = EconomyManager.Instance.CanAfford(cost);
    }

    public void RefreshAllLabels()
    {
        foreach (TowerData data in buttonLabels.Keys)
            RefreshButtonLabel(data);
    }

    public void TogglePanel()
    {
        PlacementManager.Instance.DeselectTower();
        TowerSelectionPanel.Instance.OnTowerPlacedOrCancelled();
        TowerInfoPanel.Instance.Close();
        bool isOpen = !panel.activeSelf;
        panel.SetActive(isOpen);

        if (!isOpen)
        {
            PlacementManager.Instance.DeselectTower();
            HideGrid();
        }
    }

    void Update()
    {
        // Atajos: teclas 1-9 seleccionan la torre correspondiente
        for (int i = 0; i < availableTowers.Length && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                TowerClickHandler.DeselectCurrent(); // cierra panel de info y resaltado si habia
                SelectTower(availableTowers[i]);
                break;
            }
        }
    }

    void SelectTower(TowerData data)
    {
        if (!EconomyManager.Instance.CanAfford(data.GetEffectiveCost())) return;
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