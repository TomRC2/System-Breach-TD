using UnityEngine;
using System;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;
    private int totalSpent = 0;
    public event Action<int> OnMoneyChanged;

    [Header("Config")]
    public int startingMoney = 150;

    private int currentMoney;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentMoney = startingMoney;
        OnMoneyChanged?.Invoke(currentMoney);
    }

    public bool CanAfford(int amount) => currentMoney >= amount;

    public void Earn(int amount)
    {
        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney);
        if (currentMoney >= 5000)
            AchievementManager.Instance?.RegisterRAMHoarder();
    }

    public bool Spend(int amount)
    {
        if (currentMoney < amount) return false;
        currentMoney -= amount;
        totalSpent += amount;
        OnMoneyChanged?.Invoke(currentMoney);
        return true;
    }
    public int GetTotalSpent() => totalSpent;
    public int GetMoney() => currentMoney;
}