using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public List<WaveData> waves;
    public Transform[] waypoints;
    public float timeBetweenWaves = 5f;

    public event Action<int, int> OnWaveChanged;
    public event Action<EnemyHealth> OnBossSpawned;

    public static WaveSpawner Instance;

    private int currentWave = 0;
    private int activeEnemies = 0;
    private bool spawning = false;
    private bool skipWait = false;

    [Header("UI")]
    public GameObject startButton;

    void Awake()
    {
        Instance = this;
    }

    public void BeginGame()
    {
        startButton.SetActive(false);
        StartCoroutine(StartWave());
    }

    IEnumerator StartWave()
    {
        if (currentWave >= waves.Count) yield break;

        OnWaveChanged?.Invoke(currentWave + 1, waves.Count);
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
        {
            if (currentWave >= waves.Count - 1)
                GameManager.Instance.Victory();
            else
                StartCoroutine(NextWave());
        }
    }

    void SpawnEnemy(EnemyData data)
    {
        GameObject obj = Instantiate(data.prefab, waypoints[0].position, Quaternion.identity);

        EnemyMovement movement = obj.GetComponent<EnemyMovement>();
        movement.SetWaypoints(waypoints);
        movement.speed = data.speed;

        EnemyHealth health = obj.GetComponent<EnemyHealth>();
        health.reward = data.reward;

        if (data.isBoss)
        {
            int levelNumber = GameManager.Instance != null ? GameManager.Instance.levelNumber : 1;
            health.maxHP = data.hp + data.hpScalingPerLevel * (levelNumber - 1);
        }
        else
        {
            health.maxHP = data.hp;
        }

        health.Initialize();

        if (data.isBoss)
            OnBossSpawned?.Invoke(health);

        activeEnemies++;
        health.OnDeath += OnEnemyDefeated;
        health.OnReach += OnEnemyDefeated;
    }

    void OnEnemyDefeated()
    {
        activeEnemies--;
        if (activeEnemies <= 0)
        {
            if (currentWave >= waves.Count - 1 && !spawning)
                GameManager.Instance.Victory();
            else if (!spawning)
                StartCoroutine(NextWave());
        }
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