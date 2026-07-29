using UnityEngine;
using System;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    [Header("Achievements")]
    public AchievementData[] achievements;

    public event Action OnAchievementUpdated;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Incrementa el progreso de un logro por su ID
    public void AddProgress(string achievementID, int amount = 1)
    {
        AchievementData data = GetData(achievementID);
        if (data == null) return;

        int current = GetProgress(achievementID);
        int newProgress = current + amount;
        PlayerPrefs.SetInt($"ach_progress_{achievementID}", newProgress);
        PlayerPrefs.Save();

        OnAchievementUpdated?.Invoke();
    }

    // Devuelve el progreso actual
    public int GetProgress(string achievementID)
    {
        return PlayerPrefs.GetInt($"ach_progress_{achievementID}", 0);
    }

    // Devuelve el tier actual completado (0 = ninguno)
    public int GetCurrentTier(string achievementID)
    {
        return PlayerPrefs.GetInt($"ach_tier_{achievementID}", 0);
    }

    // Devuelve si el logro single está completado
    public bool IsSingleCompleted(string achievementID)
    {
        return PlayerPrefs.GetInt($"ach_single_{achievementID}", 0) == 1;
    }

    // Completa un logro single
    public void CompleteSingle(string achievementID)
    {
        if (IsSingleCompleted(achievementID)) return;
        PlayerPrefs.SetInt($"ach_single_{achievementID}", 1);
        PlayerPrefs.Save();
        OnAchievementUpdated?.Invoke();
    }

    // Verifica si el siguiente tier está listo para claimear
    public bool CanClaim(string achievementID)
    {
        AchievementData data = GetData(achievementID);
        if (data == null || data.type == AchievementType.Single) return false;

        int currentTier = GetCurrentTier(achievementID);
        if (currentTier >= data.tiers.Length) return false;

        int progress = GetProgress(achievementID);
        return progress >= data.tiers[currentTier].requirement;
    }

    // Claimea el siguiente tier
    public void Claim(string achievementID)
    {
        if (!CanClaim(achievementID)) return;

        int currentTier = GetCurrentTier(achievementID);
        // TODO: dar recompensa data.tiers[currentTier].reward cuando esté el sistema

        PlayerPrefs.SetInt($"ach_tier_{achievementID}", currentTier + 1);
        PlayerPrefs.Save();

        OnAchievementUpdated?.Invoke();
    }

    AchievementData GetData(string achievementID)
    {
        foreach (AchievementData data in achievements)
            if (data.achievementID == achievementID) return data;
        return null;
    }

    // Accesos rápidos para los logros específicos
    public void RegisterKill() => AddProgress("virus_slayer");
    public void RegisterBossKill() => AddProgress("boss_slayer");
    public void RegisterAdWatched() => AddProgress("ads_watcher");
    public void RegisterLevelCompleted() => AddProgress("level_completitionist");
    public void RegisterTowerPlaced() => AddProgress("tower_maniac");
    public void RegisterFirstBlood() => CompleteSingle("first_blood");
    public void RegisterNoDamage() => CompleteSingle("no_damage_run");
    public void RegisterSpeedRun() => CompleteSingle("speed_runner");
    public void RegisterOverclock() => AddProgress("overclock");
    public void RegisterSell() => AddProgress("saver");
    public void RegisterFullGrid() => CompleteSingle("the_architect");
    public void RegisterMaxOut() => CompleteSingle("max_out");
    public void RegisterFullArsenal() => CompleteSingle("full_arsenal");
    public void RegisterFrugal() => CompleteSingle("frugal_engineer");
    public void RegisterRAMHoarder() => CompleteSingle("ram_hoarder");
}