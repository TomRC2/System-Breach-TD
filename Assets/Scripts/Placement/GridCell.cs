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
        placedTower.GetComponent<TowerController>().Initialize(data);

        isOccupied = true;
        UpdateVisual();
        return true;
    }

    public void RemoveTower()
    {
        if (!isOccupied) return;
        Destroy(placedTower);
        isOccupied = false;
        UpdateVisual();
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

