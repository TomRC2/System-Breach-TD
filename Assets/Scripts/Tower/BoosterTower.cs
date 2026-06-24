using System.Collections.Generic;
using UnityEngine;

public class BoosterTower : MonoBehaviour
{
    public TowerData data;
    public int currentLevel = 0;
    public List<TowerController> boostedTowers = new List<TowerController>();

    public void Initialize(TowerData towerData)
    {
        data = towerData;
        currentLevel = 0;
        // Refrescar cada 0.5s en vez de cada frame (Physics.OverlapSphere es costoso)
        InvokeRepeating(nameof(RefreshBoost), 0f, 0.5f);
    }
    public void Upgrade()
    {
        if (currentLevel >= data.levels.Length - 1) return;
        currentLevel++;
        RefreshBoost();
    }

    void ApplyBoost()
    {
        RemoveAllBoosts();

        float range = data.levels[currentLevel].range;
        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        foreach (Collider hit in hits)
        {
            TowerController tower = hit.GetComponent<TowerController>();
            if (tower == null) tower = hit.GetComponentInParent<TowerController>();
            if (tower == null) continue;
            if (tower.GetData() == null) continue;
            if (tower.GetData().towerType == TowerType.Attack && !tower.IsBoosted())
            {
                tower.ApplyBoost(data.levels[currentLevel]);
                boostedTowers.Add(tower);
            }
        }
    }

    void RemoveAllBoosts()
    {
        foreach (TowerController tower in boostedTowers)
        {
            if (tower != null)
                tower.RemoveBoost();
        }
        boostedTowers.Clear();
    }

    void RefreshBoost()
    {
        RemoveAllBoosts();
        ApplyBoost();
    }

    void OnDestroy()
    {
        RemoveAllBoosts();
    }
    public TowerData GetData() => data;
    public TowerLevel GetCurrentLevelStats() => data.levels[currentLevel];
    public int GetCurrentLevel() => currentLevel + 1;
    void OnDrawGizmosSelected()
    {
        if (data == null || data.levels.Length == 0) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.levels[currentLevel].range);
    }
}