using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHP = 100f;
    private float currentHP;

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
        Destroy(gameObject);
    }

    public void ReachComputer()
    {
        ComputerHealth computer = FindFirstObjectByType<ComputerHealth>();
        if (computer != null) computer.TakeDamage(currentHP);
        Destroy(gameObject);
    }

    public float GetCurrentHP() => currentHP;
}