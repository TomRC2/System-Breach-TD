using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectManager : MonoBehaviour
{
    [Header("Level Buttons")]
    // Asignar los 10 botones de la grilla en el Inspector, en orden
    public Button[] levelButtons;
    [Header("Lock Icon")]
    public Sprite lockSprite;
    public GameObject lockIconPrefab; // Image con el sprite del candado
    private const int TOTAL_LEVELS = 10;
    private const string KEY_UNLOCKED = "levels_unlocked"; // int: cuántos niveles desbloqueados

    void Start()
    {
        // Siempre el nivel 1 desbloqueado como mínimo
        int unlocked = Mathf.Max(PlayerPrefs.GetInt(KEY_UNLOCKED, 1), 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1; // niveles 1-10
            bool isUnlocked = levelIndex <= unlocked;

            levelButtons[i].interactable = isUnlocked;

            // Cambiar texto del botón según estado
            TMP_Text label = levelButtons[i].GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.text = isUnlocked ? $"Level {levelIndex}" : "";

            // Asignar listener con closure correcta
            if (isUnlocked)
            {
                levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex));
            }
            if (!isUnlocked)
            {
                GameObject lockIcon = new GameObject("LockIcon");
                lockIcon.transform.SetParent(levelButtons[i].transform, false);
                Image lockImage = lockIcon.AddComponent<Image>();
                lockImage.sprite = lockSprite;
                lockImage.raycastTarget = false;

                RectTransform lockRect = lockIcon.GetComponent<RectTransform>();
                lockRect.anchorMin = new Vector2(0.5f, 0.5f);
                lockRect.anchorMax = new Vector2(0.5f, 0.5f);
                lockRect.anchoredPosition = Vector2.zero;
                lockRect.sizeDelta = new Vector2(40f, 40f); // ajustá el tamaño
            }
        }
    }

    void LoadLevel(int levelNumber)
    {
        // Convención de nombre de escena: "Level1", "Level2", etc.
        SceneManager.LoadScene("Level" + levelNumber);
        AudioManager.Instance.PlayGameplayMusic();
    }

    // Llamar esto al completar un nivel para desbloquear el siguiente
    public static void UnlockNextLevel(int completedLevel)
    {
        int current = PlayerPrefs.GetInt(KEY_UNLOCKED, 1);
        int next = completedLevel + 1;
        if (next > current && next <= TOTAL_LEVELS)
        {
            PlayerPrefs.SetInt(KEY_UNLOCKED, next);
            PlayerPrefs.Save();
        }
    }

    public void Back()
    {
        MenuManager.Instance.GoHome();
    }
}
