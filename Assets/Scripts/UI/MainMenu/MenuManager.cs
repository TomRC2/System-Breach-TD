using Unity.VisualScripting;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [Header("Panels")]
    public GameObject panelMainMenu;
    public GameObject panelLevelSelect;
    public GameObject panelOptions;
    public GameObject panelCredits;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CameraMenuController cam = CameraMenuController.Instance;
        cam.OnArriveHome += () => OpenPanel(panelMainMenu);
        cam.OnArriveLevelSelect += () => OpenPanel(panelLevelSelect);
        cam.OnArriveOptions += () => OpenPanel(panelOptions);
        cam.OnArriveCredits += () => OpenPanel(panelCredits);
    }

    public void GoHome()
    {
        CloseAllPanels();
        CameraMenuController.Instance.GoHome();
    }

    void OpenPanel(GameObject panel)
    {
        CloseAllPanels();
        panel.SetActive(true);
    }

    public void CloseAllPanels()
    {
        panelMainMenu.SetActive(false);
        panelLevelSelect.SetActive(false);
        panelOptions.SetActive(false);
        panelCredits.SetActive(false);
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
