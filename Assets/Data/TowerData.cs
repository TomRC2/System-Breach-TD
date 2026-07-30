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

    public float attackSpeedBonus;
    public float damageBonus;
    public float rangeBonus;

    public int moneyPerWave;

    [Header("Efectos opcionales (0 = desactivado)")]
    [Tooltip("Radio de danio en area al impactar")]
    public float splashRadius;
    [Tooltip("Fraccion de ralentizacion aplicada al enemigo (0.3 = 30% mas lento)")]
    [Range(0f, 0.9f)] public float slowAmount;
    [Tooltip("Duracion de la ralentizacion en segundos")]
    public float slowDuration;
}

[CreateAssetMenu(fileName = "TowerData", menuName = "SystemBreach/Tower Data")]
public class TowerData : ScriptableObject
{
    public string towerName;
    public GameObject prefab;
    public int cost;
    public float critMultiplier = 2f;
    public TowerType towerType = TowerType.Attack;
    public AudioClip attackSFX;
    [TextArea] public string description;
    public TowerLevel[] levels;

    public int GetEffectiveCost()
    {
        if (towerType != TowerType.Farm) return cost;
        int effective = Mathf.RoundToInt(cost * Mathf.Pow(1.5f, FarmTower.farmCount));
        return effective;
    }
}