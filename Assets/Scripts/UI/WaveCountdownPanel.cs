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
            countdownText.text = $"Next wave in {Mathf.CeilToInt(remaining)}s";
            remaining -= Time.deltaTime;
            yield return null;
        }

        Hide();
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