using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GlobalButtonSound : MonoBehaviour
{
    public static GlobalButtonSound Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            HookButtons();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HookButtons();
    }

    void HookButtons()
    {
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Button btn in allButtons)
        {
            btn.onClick.RemoveListener(PlaySound);
            btn.onClick.AddListener(PlaySound);
        }
    }

    void PlaySound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonSound();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}