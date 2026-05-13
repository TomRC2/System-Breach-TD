using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemyGroup
{
    public EnemyData enemyData;
    public int count;
    public float spawnInterval;
}

[CreateAssetMenu(fileName = "WaveData", menuName = "SystemBreach/Wave Data")]
public class WaveData : ScriptableObject
{
    public List<EnemyGroup> groups;
    public float timeBetweenGroups;
}