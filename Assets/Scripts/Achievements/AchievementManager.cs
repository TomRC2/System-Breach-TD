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

    public void AddProgress(string achievementID, int amount = 1)
    {
        AchievementData data = GetData(achievementID);
        if (data == null) return;

        int current = GetProgress(achievementID);
        int newProgress = current + amount;
        PlayerPrefs.SetInt($"ach_progress_{achievementID}", newProgress);
        PlayerPrefs.Save();

        int currentTier = GetCurrentTier(achievementID);
        if (currentTier < data.tiers.Length && newProgress >= data.tiers[currentTier].requirement)
        {
            bool alreadyNotified = PlayerPrefs.GetInt($"ach_notified_{achievementID}_{currentTier}", 0) == 1;
            if (!alreadyNotified)
            {
                PlayerPrefs.SetInt($"ach_notified_{achievementID}_{currentTier}", 1);
                PlayerPrefs.SetInt("ach_new_notification", 1);
                PlayerPrefs.Save();
                if (AchievementNotification.Instance != null)
                    AchievementNotification.Instance.Show(data, currentTier);
            }
        }

        OnAchievementUpdated?.Invoke();
    }

    public int GetProgress(string achievementID)
    {
        return PlayerPrefs.GetInt($"ach_progress_{achievementID}", 0);
    }

    public int GetCurrentTier(string achievementID)
    {
        return PlayerPrefs.GetInt($"ach_tier_{achievementID}", 0);
    }

    public bool IsSingleCompleted(string achievementID)
    {
        return PlayerPrefs.GetInt($"ach_single_{achievementID}", 0) == 1;
    }

    public void CompleteSingle(string achievementID)
    {
        if (IsSingleCompleted(achievementID)) return;

        AchievementData data = GetData(achievementID);
        PlayerPrefs.SetInt($"ach_single_{achievementID}", 1);
        PlayerPrefs.SetInt("ach_new_notification", 1);
        PlayerPrefs.Save();

        if (AchievementNotification.Instance != null)
            AchievementNotification.Instance.Show(data);

        OnAchievementUpdated?.Invoke();
    }

    public bool CanClaim(string achievementID)
    {
        AchievementData data = GetData(achievementID);
        if (data == null || data.type == AchievementType.Single) return false;

        int currentTier = GetCurrentTier(achievementID);
        if (currentTier >= data.tiers.Length) return false;

        int progress = GetProgress(achievementID);
        return progress >= data.tiers[currentTier].requirement;
    }

    public void Claim(string achievementID)
    {
        if (!CanClaim(achievementID))return;

        AchievementData data = GetData(achievementID);
        int currentTier = GetCurrentTier(achievementID);

        PlayerPrefs.SetInt($"ach_tier_{achievementID}", currentTier + 1);
        PlayerPrefs.SetInt("ach_new_notification", 1);
        PlayerPrefs.Save();

        if (AchievementNotification.Instance != null)
            AchievementNotification.Instance.Show(data, currentTier);

        OnAchievementUpdated?.Invoke();
    }

    AchievementData GetData(string achievementID)
    {
        foreach (AchievementData data in achievements)
            if (data.achievementID == achievementID) return data;
        return null;
    }

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