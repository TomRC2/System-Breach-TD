using UnityEngine;
using System;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;

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
    }

    public bool Spend(int amount)
    {
        if (!CanAfford(amount)) return false;
        currentMoney -= amount;
        OnMoneyChanged?.Invoke(currentMoney);
        return true;
    }

    public int GetMoney() => currentMoney;
}