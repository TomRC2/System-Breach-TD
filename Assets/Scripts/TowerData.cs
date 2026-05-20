using UnityEngine;

[CreateAssetMenu(fileName = "TowerData", menuName = "SystemBreach/Tower Data")]
public class TowerData : ScriptableObject
{
    public string towerName;
    public GameObject prefab;
    public int cost;
    public float damage;
    public float attackSpeed;
    public float range;
    public float critChance;
    public float critMultiplier = 2f;
}