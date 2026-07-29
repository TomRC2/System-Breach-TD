using UnityEngine;

public class AchievementsPanel : MonoBehaviour
{
    [Header("Referencias")]
    public AchievementManager achievementManager;
    public Transform listContent;
    public GameObject achievementUIPrefab;

    private AchievementUI[] achievementUIs;

    void OnEnable()
    {
        BuildList();
        if (AchievementManager.Instance != null)
            AchievementManager.Instance.OnAchievementUpdated += RefreshAll;
    }

    void OnDisable()
    {
        if (AchievementManager.Instance != null)
            AchievementManager.Instance.OnAchievementUpdated -= RefreshAll;
    }

    void BuildList()
    {
        foreach (Transform child in listContent)
            Destroy(child.gameObject);

        if (AchievementManager.Instance == null) return;

        AchievementData[] achievements = AchievementManager.Instance.achievements;
        achievementUIs = new AchievementUI[achievements.Length];

        for (int i = 0; i < achievements.Length; i++)
        {
            GameObject go = Instantiate(achievementUIPrefab, listContent);
            AchievementUI ui = go.GetComponent<AchievementUI>();
            ui.Setup(achievements[i]);
            achievementUIs[i] = ui;
        }
    }

    void RefreshAll()
    {
        if (achievementUIs == null) return;
        foreach (AchievementUI ui in achievementUIs)
            ui.Refresh();
    }
}