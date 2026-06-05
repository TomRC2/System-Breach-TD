using UnityEngine;

public enum TowerType { Attack, Booster, Farm }

[System.Serializable]
public class TowerLevel
{
    public float damage;
    public float attackSpeed;
    public float range;
    public float critChance;
    public int upgradeCost;

    // Booster
    public float attackSpeedBonus;
    public float damageBonus;
    public float rangeBonus;

    // Farm
    public int moneyPerWave;
}

[CreateAssetMenu(fileName = "TowerData", menuName = "SystemBreach/Tower Data")]
public class TowerData : ScriptableObject
{
    public string towerName;
    public GameObject prefab;
    public int cost;
    public float critMultiplier = 2f;
    public TowerType towerType = TowerType.Attack;

    public TowerLevel[] levels;
}