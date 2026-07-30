using UnityEngine;

public class GridCell : MonoBehaviour
{
    public bool isOccupied = false;
    private GameObject placedTower;

    private Color availableColor = new Color(0f, 1f, 0f, 0.3f);
    private Color occupiedColor = new Color(1f, 0f, 0f, 0.3f);
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        UpdateVisual();
    }

    public bool PlaceTower(GameObject towerPrefab, TowerData data)
    {
        if (isOccupied) return false;

        placedTower = Instantiate(towerPrefab, transform.position, Quaternion.identity);

        if (data.towerType == TowerType.Attack)
            placedTower.GetComponent<TowerController>().Initialize(data);
        else if (data.towerType == TowerType.Booster)
            placedTower.GetComponent<BoosterTower>().Initialize(data);
        else if (data.towerType == TowerType.Farm)
            placedTower.GetComponent<FarmTower>().Initialize(data);

        // Animacion de aparicion (pop de escala)
        placedTower.AddComponent<TowerScaleFX>().PlaySpawn();

        isOccupied = true;
        UpdateVisual();
        return true;
    }
    public TowerData GetPlacedTowerData()
    {
        if (!isOccupied || placedTower == null) return null;
        TowerController tc = placedTower.GetComponent<TowerController>();
        if (tc != null) return tc.GetData();
        BoosterTower bt = placedTower.GetComponent<BoosterTower>();
        if (bt != null) return bt.GetData();
        FarmTower ft = placedTower.GetComponent<FarmTower>();
        if (ft != null) return ft.GetData();
        return null;
    }
    public void FreeCellAndDestroy()
    {
        if (!isOccupied) return;
        if (placedTower != null)
        {
            placedTower.SetActive(false); // dispara OnDisable de inmediato (Destroy es diferido)
            Destroy(placedTower);
        }
        placedTower = null;
        isOccupied = false;
        UpdateVisual();
    }

    public bool IsOccupiedBy(GameObject tower)
    {
        if (!isOccupied || placedTower == null) return false;
        return placedTower == tower || placedTower == tower.transform.root.gameObject;
    }

    void UpdateVisual()
    {
        if (rend != null)
            rend.material.color = isOccupied ? occupiedColor : availableColor;
    }

    void OnMouseEnter()
    {
        if (!isOccupied)
            rend.material.color = new Color(1f, 1f, 0f, 0.4f);
    }

    void OnMouseExit()
    {
        UpdateVisual();
    }
}