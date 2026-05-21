using UnityEngine;

public class ComputerHealth : MonoBehaviour
{
    public float maxHP = 1000f;
    private float currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        Debug.Log($"PC HP: {currentHP}");
        if (currentHP <= 0) GameOver();
    }

    void GameOver()
    {
        Debug.Log("GAME OVER");
    }
}