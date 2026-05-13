using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveSpawner : MonoBehaviour
{
    public List<WaveData> waves;
    public Transform[] waypoints;
    public float timeBetweenWaves = 5f;

    private int currentWave = 0;
    private int activeEnemies = 0;
    private bool spawning = false;

    void Start()
    {
        StartCoroutine(StartWave());
    }

    IEnumerator StartWave()
    {
        if (currentWave >= waves.Count)
        {
            Debug.Log("NIVEL COMPLETADO");
            yield break;
        }

        spawning = true;
        WaveData wave = waves[currentWave];

        foreach (EnemyGroup group in wave.groups)
        {
            for (int i = 0; i < group.count; i++)
            {
                SpawnEnemy(group.enemyData);
                yield return new WaitForSeconds(group.spawnInterval);
            }
            yield return new WaitForSeconds(wave.timeBetweenGroups);
        }

        spawning = false;
    }

    void SpawnEnemy(EnemyData data)
    {
        GameObject obj = Instantiate(data.prefab, waypoints[0].position, Quaternion.identity);

        EnemyMovement movement = obj.GetComponent<EnemyMovement>();
        movement.SetWaypoints(waypoints);
        movement.speed = data.speed;

        EnemyHealth health = obj.GetComponent<EnemyHealth>();
        health.maxHP = data.hp;
        health.reward = data.reward;

        activeEnemies++;
        health.OnDeath += OnEnemyDefeated;
        health.OnReach += OnEnemyDefeated;
    }

    void OnEnemyDefeated()
    {
        activeEnemies--;
        if (!spawning && activeEnemies <= 0)
            StartCoroutine(NextWave());
    }

    IEnumerator NextWave()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        currentWave++;
        StartCoroutine(StartWave());
    }
}