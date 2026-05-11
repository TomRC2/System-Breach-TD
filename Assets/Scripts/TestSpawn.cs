using UnityEngine;

public class TestSpawn : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] waypoints;

    void Start()
    {
        GameObject enemy = Instantiate(enemyPrefab, waypoints[0].position, Quaternion.identity);
        enemy.GetComponent<EnemyMovement>().SetWaypoints(waypoints);
    }
}