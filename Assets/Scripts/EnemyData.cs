using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "SystemBreach/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public GameObject prefab;
    public float hp;
    public float speed;
    public float reward;
}