using UnityEngine;

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
                    if (!EconomyManager.Instance.CanAfford(selectedTower.cost))
                    {
                        Debug.Log("Dinero insuficiente");
                        return;
                    }

                    bool placed = cell.PlaceTower(selectedTower.prefab, selectedTower);
                    if (placed)
                    {
                        AudioManager.Instance?.PlaySFX(AudioManager.Instance.placeTowerSFX);
                        EconomyManager.Instance.Spend(selectedTower.cost);
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