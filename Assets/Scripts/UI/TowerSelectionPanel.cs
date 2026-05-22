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
    }

    void GenerateButtons()
    {
        foreach (TowerData data in availableTowers)
        {
            GameObject btn = Instantiate(towerButtonPrefab, buttonContainer);

            TMP_Text label = btn.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = data.towerName;

            TowerData captured = data;
            btn.GetComponent<Button>().onClick.AddListener(() => SelectTower(captured));
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