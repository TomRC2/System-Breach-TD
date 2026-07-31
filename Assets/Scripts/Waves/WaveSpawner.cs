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
        GameManager.Instance.StartLevelTimer();
    }

    IEnumerator StartWave()
    {
        if (currentWave >= waves.Count) yield break;

        OnWaveChanged?.Invoke(currentWave + 1, waves.Count);
        WaveBanner.Show(currentWave + 1, waves.Count);
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

        CheckWaveCleared();

        if (Time.timeScale >= 2f)
            AchievementManager.Instance?.RegisterOverclock();
    }

    // Atajo de teclado: Espacio inicia el juego o salta la espera entre oleadas
    void Update()
    {
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (startButton != null && startButton.activeSelf)
                BeginGame();
            else
                WaveCountdownPanel.Instance?.SkipIfVisible();
        }
    }

    // Logica unificada de fin de oleada (antes duplicada en dos lugares)
    void CheckWaveCleared()
    {
        if (activeEnemies > 0 || spawning) return;

        if (currentWave >= waves.Count - 1)
            GameManager.Instance.Victory();
        else
            StartCoroutine(NextWave());
    }

    void SpawnEnemy(EnemyData data)
    {
        GameObject obj = Instantiate(data.prefab, waypoints[0].position, Quaternion.identity);

        EnemyMovement movement = obj.GetComponent<EnemyMovement>();
        movement.SetWaypoints(waypoints);
        movement.speed = data.speed;

        EnemyHealth health = obj.GetComponent<EnemyHealth>();
        health.reward = data.reward;
        health.isBoss = data.isBoss;
        health.displayName = data.enemyName;

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
        {
            OnBossSpawned?.Invoke(health);
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.bossSpawnSFX);
        }

        activeEnemies++;
        health.OnDeath += OnEnemyDefeated;
        health.OnReach += OnEnemyDefeated;
    }

    void OnEnemyDefeated()
    {
        activeEnemies--;
        CheckWaveCleared();
    }

    public void SkipWaitTime()
    {
        skipWait = true;
    }

    IEnumerator NextWave()
    {
        skipWait = false;
        WaveCountdownPanel.Instance?.StartCountdown(timeBetweenWaves);
        float elapsed = 0f;
        while (elapsed < timeBetweenWaves && !skipWait)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        WaveCountdownPanel.Instance?.Hide();
        currentWave++;
        StartCoroutine(StartWave());
    }
}