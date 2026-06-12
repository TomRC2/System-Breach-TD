using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "SystemBreach/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public GameObject prefab;
    public float hp;
    public float speed;
    public float reward;

    [Header("Boss")]
    public bool isBoss = false;
    public float hpScalingPerLevel = 200f;
}