using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Poner en un Button dentro del panel de Options para alternar EN/ES.
// El campo buttonLabel es opcional: si se asigna, muestra "EN" o "ES".
public class LanguageToggle : MonoBehaviour
{
    public TMP_Text buttonLabel;

    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnClick);

        Refresh();
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged += Refresh;
    }

    void OnDestroy()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= Refresh;
    }

    void OnClick()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.ToggleLanguage();
    }

    void Refresh()
    {
        if (buttonLabel == null || LocalizationManager.Instance == null) return;
        buttonLabel.text = LocalizationManager.Instance.CurrentLanguage == LocalizationManager.Language.English ? "EN" : "ES";
    }
}
