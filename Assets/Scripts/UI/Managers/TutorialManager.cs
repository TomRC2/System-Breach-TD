using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class TutorialSlide
{
    public string title;
    [TextArea] public string description;
    public Sprite image;
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Slides")]
    public TutorialSlide[] slides;

    [Header("UI References")]
    public Image slideImage;
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text slideCounterText;
    public Button nextButton;
    public Button prevButton;
    public Button startButton; // aparece solo en el ultimo slide

    [Header("Typewriter")]
    public float typingSpeed = 0.03f;
    [Header("Panel")]
    public GameObject tutorialPanel;
    [Header("Scene")]
    public string levelSceneName = "Level1";

    private int currentSlide = 0;
    private bool isTyping = false;
    private string fullText = "";
    private Coroutine typingCoroutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        nextButton.onClick.AddListener(OnNextClicked);
        prevButton.onClick.AddListener(OnPrevClicked);
        startButton.onClick.AddListener(StartLevel);

        ShowSlide(0);
    }

    void OnNextClicked()
    {
        if (isTyping)
        {
            FinishTyping();
            return;
        }

        if (currentSlide < slides.Length - 1)
        {
            currentSlide++;
            ShowSlide(currentSlide);
        }
    }

    void OnPrevClicked()
    {
        if (isTyping) FinishTyping();

        if (currentSlide > 0)
        {
            currentSlide--;
            ShowSlide(currentSlide);
        }
    }

    void ShowSlide(int index)
    {
        TutorialSlide slide = slides[index];

        titleText.text = slide.title;
        slideCounterText.text = $"{index + 1} / {slides.Length}";

        if (slide.image != null)
            slideImage.sprite = slide.image;

        // Typewriter
        fullText = slide.description;
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(fullText));

        // Botones
        prevButton.gameObject.SetActive(index > 0);
        nextButton.gameObject.SetActive(index < slides.Length - 1);
        startButton.gameObject.SetActive(index == slides.Length - 1);
    }

    void FinishTyping()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        descriptionText.text = fullText;
        isTyping = false;
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        descriptionText.text = "";
        foreach (char c in text)
        {
            descriptionText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        isTyping = false;
    }

    void StartLevel()
    {
        PlayerPrefs.SetInt("tutorial_done", 1);
        PlayerPrefs.Save();
        Time.timeScale = 1f;
        tutorialPanel.SetActive(false);
    }


    // Llamado desde el botón de pausa in-game
    public static bool IsTutorialDone()
    {
        return PlayerPrefs.GetInt("tutorial_done", 0) == 1;
    }
}
