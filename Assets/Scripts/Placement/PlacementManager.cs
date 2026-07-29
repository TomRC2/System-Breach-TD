using UnityEngine;
using System.Linq;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;
    private TowerData selectedTower;

    void Awake()
    {
        Instance = this;
    }

    public void SelectTower(TowerData data)
    {
        selectedTower = data;
    }

    public void DeselectTower()
    {
        selectedTower = null;
    }

    void Update()
    {
        if (selectedTower == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GridCell cell = hit.collider.GetComponent<GridCell>();
                if (cell != null)
                {
                    int cost = selectedTower.GetEffectiveCost();

                    if (!EconomyManager.Instance.CanAfford(cost))
                    {
                        Debug.Log("Dinero insuficiente");
                        return;
                    }

                    bool placed = cell.PlaceTower(selectedTower.prefab, selectedTower);
                    if (placed)
                    {
                        AudioManager.Instance?.PlaySFX(AudioManager.Instance.placeTowerSFX);
                        EconomyManager.Instance.Spend(cost);
                        TowerSelectionPanel.Instance.RefreshAllLabels();

                        AchievementManager.Instance?.RegisterTowerPlaced();

                        if (TowerSelectionPanel.Instance.allCells.All(c => c.isOccupied))
                            AchievementManager.Instance?.RegisterFullGrid();

                        bool hasAttack = false, hasBooster = false, hasFarm = false;
                        foreach (GridCell c in TowerSelectionPanel.Instance.allCells)
                        {
                            if (!c.isOccupied) continue;
                            TowerData data = c.GetPlacedTowerData();
                            if (data == null) continue;
                            if (data.towerType == TowerType.Attack) hasAttack = true;
                            if (data.towerType == TowerType.Booster) hasBooster = true;
                            if (data.towerType == TowerType.Farm) hasFarm = true;
                        }
                        if (hasAttack && hasBooster && hasFarm)
                            AchievementManager.Instance?.RegisterFullArsenal();

                        DeselectTower();
                        TowerSelectionPanel.Instance.OnTowerPlacedOrCancelled();
                        TowerSelectionPanel.Instance.panel.SetActive(true);
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            DeselectTower();
            TowerSelectionPanel.Instance.OnTowerPlacedOrCancelled();
        }
    }
}