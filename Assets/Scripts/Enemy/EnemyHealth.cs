using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    public float maxHP = 100f;
    public float reward = 10f;
    public bool isBoss = false;

    private float currentHP;
    private bool initialized = false;
    private bool isDead = false;

    public Action OnDeath;
    public Action OnReach;

    void Start()
    {
        if (!initialized)
            Initialize();
    }

    public void Initialize()
    {
        currentHP = maxHP;
        initialized = true;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        currentHP -= amount;
        if (currentHP <= 0) Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        OnDeath?.Invoke();

        if (EconomyManager.Instance != null)
            EconomyManager.Instance.Earn((int)reward);

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.RegisterKill(reward, isBoss);

        Destroy(gameObject);
    }

    public void ReachComputer()
    {
        if (isDead) return;
        isDead = true;

        ComputerHealth computer = FindFirstObjectByType<ComputerHealth>();
        if (computer != null) computer.TakeDamage(maxHP);

        OnReach?.Invoke();
        Destroy(gameObject);
    }

    public float GetCurrentHP() => currentHP;
}