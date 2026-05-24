using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public List<WaveData> waves;
    public Transform[] waypoints;
    public float timeBetweenWaves = 5f;
    public event Action<int, int> OnWaveChanged;
    public static WaveSpawner Instance;


    private int currentWave = 0;
    private int activeEnemies = 0;
    private bool spawning = false;
    private bool skipWait = false;
    void Start()
    {
        StartCoroutine(StartWave());
    }
    void Awake()
    {
        Instance = this;
    }
    IEnumerator StartWave()
    {
        OnWaveChanged?.Invoke(currentWave + 1, waves.Count);
        if (currentWave >= waves.Count)
        {
            GameManager.Instance.Victory();
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

        if (activeEnemies <= 0)
            StartCoroutine(NextWave());
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

    public void SkipWaitTime()
    {
        skipWait = true;
    }

    IEnumerator NextWave()
    {
        skipWait = false;
        WaveCountdownPanel.Instance.StartCountdown(timeBetweenWaves);

        float elapsed = 0f;
        while (elapsed < timeBetweenWaves && !skipWait)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        WaveCountdownPanel.Instance.Hide();
        currentWave++;
        StartCoroutine(StartWave());
    }


}