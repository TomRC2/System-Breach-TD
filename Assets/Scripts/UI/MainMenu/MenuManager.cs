using System.Collections.Generic;
using UnityEngine;
public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [Header("Panels")]
    public GameObject panelMainMenu;
    public GameObject panelLevelSelect;
    public GameObject panelOptions;
    public GameObject panelCredits;
    public GameObject panelInfo;
    [Header("Info Sub-panels")]
    public GameObject panelAchievements;
    public GameObject panelGallery;
    [Header("Achievement Badge")]
    public GameObject achievementBadge;
    public GameObject infoBadgeAchievement;

    private Stack<GameObject> panelHistory = new Stack<GameObject>();
    void Awake()
    {
        Instance = this;
        AudioManager.Instance.PlayMenuMusic();
    }

    void Start()
    {
        if (achievementBadge != null)
            achievementBadge.SetActive(PlayerPrefs.GetInt("ach_new_notification", 0) == 1);
        bool hasNew = PlayerPrefs.GetInt("ach_new_notification", 0) == 1;
        if (achievementBadge != null) achievementBadge.SetActive(hasNew);
        if (infoBadgeAchievement != null) infoBadgeAchievement.SetActive(hasNew);
        CameraMenuController cam = CameraMenuController.Instance;
        cam.OnArriveHome += () => OpenPanel(panelMainMenu);
        cam.OnArriveLevelSelect += () => OpenPanel(panelLevelSelect);
        cam.OnArriveOptions += () => OpenPanel(panelOptions);
        cam.OnArriveInfo += () => OpenPanel(panelInfo);
    }
    public void ShowAchievements()
    {
        PlayerPrefs.SetInt("ach_new_notification", 0);
        PlayerPrefs.Save();
        if (infoBadgeAchievement != null) infoBadgeAchievement.SetActive(false);
        if (achievementBadge != null) achievementBadge.SetActive(false);
        OpenPanel(panelAchievements);
    }

    public void ShowGallery()
    {
        OpenPanel(panelGallery);
    }
    public void ToggleCredits()
    {
        panelCredits.SetActive(!panelCredits.activeSelf);
    }
    public void GoHome()
    {
        CloseAllPanels();
        CameraMenuController.Instance.GoHome();
    }

    void OpenPanel(GameObject panel)
    {
        foreach (var p in new[] { panelMainMenu, panelOptions, panelLevelSelect, panelInfo, panelAchievements, panelGallery })
            if (p.activeSelf) panelHistory.Push(p);

        CloseAllPanels();
        panel.SetActive(true);
    }
    public void GoBack()
    {
        if (panelHistory.Count == 0)
        {
            CameraMenuController.Instance.GoHome();
            return;
        }

        CloseAllPanels();
        GameObject prev = panelHistory.Pop();
        prev.SetActive(true);
    }
    public void CloseAllPanels()
    {
        panelMainMenu.SetActive(false);
        panelLevelSelect.SetActive(false);
        panelOptions.SetActive(false);
        panelCredits.SetActive(false);
        panelInfo.SetActive(false);
        panelAchievements.SetActive(false);
        panelGallery.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
