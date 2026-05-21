using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    public float maxHP = 100f;
    public float reward = 10f;
    private float currentHP;

    public Action OnDeath;
    public Action OnReach;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        if (currentHP <= 0) Die();
    }

    void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    public void ReachComputer()
    {
        ComputerHealth computer = FindFirstObjectByType<ComputerHealth>();
        if (computer != null) computer.TakeDamage(currentHP);
        OnReach?.Invoke();
        Destroy(gameObject);
    }

    public float GetCurrentHP() => currentHP;
}