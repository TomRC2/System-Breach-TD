using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementNotification : MonoBehaviour
{
    public static AchievementNotification Instance;

    [Header("UI")]
    public GameObject notificationPanel;
    public Image iconImage;
    public TMP_Text achievementNameText;
    public TMP_Text tierText;
    [Header("Settings")]
    public float displayDuration = 3f;
    public float fadeDuration = 0.3f;

    private CanvasGroup canvasGroup;
    private Coroutine currentCoroutine;

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
        canvasGroup = notificationPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = notificationPanel.AddComponent<CanvasGroup>();

        notificationPanel.SetActive(false);
    }

    public void Show(AchievementData data, int tier = -1)
    {
        if (iconImage != null && data.icon != null)
            iconImage.sprite = data.icon;

        if (achievementNameText != null)
            achievementNameText.text = data.achievementName;
        Debug.Log($"Show notification: {data.achievementName}");
        if (tierText != null)
        {
            if (tier >= 0 && tier < data.tiers.Length)
            {
                tierText.gameObject.SetActive(true);
                tierText.text = $"Tier {data.tiers[tier].label} unlocked!";
            }
            else
            {
                tierText.gameObject.SetActive(true);
                tierText.text = "Unlocked!";
            }
        }

        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(ShowRoutine());
    }

    IEnumerator ShowRoutine()
    {
        notificationPanel.SetActive(true);
        canvasGroup.alpha = 0f;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        yield return new WaitForSecondsRealtime(displayDuration);

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        notificationPanel.SetActive(false);
    }
}