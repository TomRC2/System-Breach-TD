using UnityEngine;

public class TestPlacement : MonoBehaviour
{
    public TowerData towerToPlace;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            PlacementManager.Instance.SelectTower(towerToPlace);
    }
}