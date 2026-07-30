using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    public float maxHP = 100f;
    public float reward = 10f;
    public bool isBoss = false;
    public string displayName = "";

    private float currentHP;
    private bool initialized = false;
    private bool isDead = false;

    [Header("Combat Visuals")]
    public EnemyHealthBar healthBar;
    public GameObject damageTextPrefab;

    public Action OnDeath;
    public Action OnReach;

    void Start()
    {
        currentHP = maxHP;
        if (healthBar != null)
            healthBar.Setup(transform, maxHP);
    }

    public void Initialize()
    {
        currentHP = maxHP;
        initialized = true;
    }

    public void TakeDamage(float amount, bool isCrit = false)
    {
        if (isDead) return;
        currentHP -= amount;

        if (healthBar != null)
            healthBar.UpdateHP(currentHP);

        if (damageTextPrefab != null && OptionsManager.IsDamageTextEnabled())
        {
            GameObject go = Instantiate(damageTextPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            go.GetComponent<DamageText>().Setup(amount, isCrit, transform.position + Vector3.up * 0.5f);
        }

        if (currentHP <= 0) Die();
    }

    void Die()
    {

        if (isDead) return;
        isDead = true;

        OnDeath?.Invoke();

        if (EconomyManager.Instance != null)
            EconomyManager.Instance.Earn((int)reward);
        if (isBoss == true)
        {
            AchievementManager.Instance.RegisterBossKill();
        }
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.RegisterKill(reward, isBoss);
        PlayerPrefs.SetInt($"enemy_discovered_{displayName}", 1);
        PlayerPrefs.Save();
        AchievementManager.Instance.RegisterKill();
        AchievementManager.Instance?.RegisterFirstBlood();
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