using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
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

        if (isOpen)
        {
            PanelFX.Show(panel);
        }
        else
        {
            panel.SetActive(false);
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

    private Dictionary<GridCell, Vector3> cellBaseScales = new Dictionary<GridCell, Vector3>();

    public void ShowGrid()
    {
        // centro del grid para expandir la onda desde ahi
        Vector3 center = Vector3.zero;
        int count = 0;
        foreach (GridCell cell in allCells)
        {
            center += cell.transform.position;
            count++;
        }
        if (count > 0) center /= count;

        // distancia maxima para normalizar los retrasos
        float maxDist = 0.01f;
        foreach (GridCell cell in allCells)
        {
            float d = Vector3.Distance(cell.transform.position, center);
            if (d > maxDist) maxDist = d;
        }

        // onda rapida del centro hacia afuera (~0.1s en total + pop de 0.08s)
        const float TOTAL_WAVE_TIME = 0.1f;
        foreach (GridCell cell in allCells)
        {
            if (cell.isOccupied) continue;

            if (!cellBaseScales.ContainsKey(cell))
                cellBaseScales[cell] = cell.transform.localScale;

            float delay = Vector3.Distance(cell.transform.position, center) / maxDist * TOTAL_WAVE_TIME;
            cell.gameObject.SetActive(true);
            StartCoroutine(PopCell(cell.transform, cellBaseScales[cell], delay));
        }
    }

    // Aparicion en cascada: cada celda hace un pequenio pop con retraso escalonado
    IEnumerator PopCell(Transform cell, Vector3 baseScale, float delay)
    {
        cell.localScale = Vector3.zero;
        float elapsed = 0f;
        while (elapsed < delay)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        float duration = 0.08f;
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            cell.localScale = baseScale * (t * t * (3f - 2f * t));
            yield return null;
        }
        cell.localScale = baseScale;
    }

    public void HideGrid()
    {
        StopAllCoroutines();
        foreach (GridCell cell in allCells)
        {
            if (cellBaseScales.ContainsKey(cell))
                cell.transform.localScale = cellBaseScales[cell];
            cell.gameObject.SetActive(false);
        }
    }
}