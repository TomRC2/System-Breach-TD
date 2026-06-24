using System;
using UnityEngine;

public class ComputerHealth : MonoBehaviour
{
    public float maxHP = 1000f;
    private float currentHP;
    public event Action<float, float> OnHPChanged;
    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        currentHP = Mathf.Max(currentHP - amount, 0f);
        if (currentHP <= 0) GameOver();
        OnHPChanged?.Invoke(currentHP, maxHP);
    }

    void GameOver()
    {
        GameManager.Instance.GameOver();
    }
}