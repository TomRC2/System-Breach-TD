using UnityEngine;

[System.Serializable]
public class TowerLevel
{
    public float damage;
    public float attackSpeed;
    public float range;
    public float critChance;
    public int upgradeCost; 
}

[CreateAssetMenu(fileName = "TowerData", menuName = "SystemBreach/Tower Data")]
public class TowerData : ScriptableObject
{
    public string towerName;
    public GameObject prefab;
    public int cost;  
    public float critMultiplier = 2f;

    public TowerLevel[] levels; 
}