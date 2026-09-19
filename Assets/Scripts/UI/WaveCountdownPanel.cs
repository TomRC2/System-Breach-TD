using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveCountdownPanel : MonoBehaviour
{
    public static WaveCountdownPanel Instance;

    [Header("Panel")]
    public GameObject panel;

    [Header("UI")]
    public TMP_Text countdownText;
    public Button skipButton;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        panel.SetActive(false);
        skipButton.onClick.AddListener(Skip);
    }

    public void StartCountdown(float duration)
    {
        if (OptionsManager.IsAutoSkipEnabled())
        {
            WaveSpawner.Instance.SkipWaitTime();
            return;
        }

        panel.SetActive(true);
        StartCoroutine(CountdownRoutine(duration));
    }

    System.Collections.IEnumerator CountdownRoutine(float duration)
    {
        float remaining = duration;

        while (remaining > 0f)
        {
            countdownText.text = string.Format(LocalizationManager.Tr("Next wave in {0}s"), Mathf.CeilToInt(remaining));
            remaining -= Time.unscaledDeltaTime;
            yield return null;
        }

        Hide();
    }

    // Llamado por el atajo de teclado (Espacio)
    public void SkipIfVisible()
    {
        if (panel != null && panel.activeSelf)
            Skip();
    }

    public void Skip()
    {
        StopAllCoroutines();
        Hide();
        WaveSpawner.Instance.SkipWaitTime();
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}