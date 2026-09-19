using UnityEngine;
using TMPro;

// Poner este componente en cualquier TMP_Text estatico del menu/HUD (botones,
// titulos, labels). Al arrancar toma el texto que ya esta puesto en el
// Inspector como "clave" en ingles, y lo reemplaza por la traduccion cuando
// corresponda. No hace falta escribir la key a mano: dejala vacia y se
// completa sola con el texto actual del label.
[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    [Tooltip("Texto en ingles que sirve de clave. Si se deja vacio, se toma el texto actual del label.")]
    [TextArea] public string key;

    private TMP_Text label;

    void Awake()
    {
        label = GetComponent<TMP_Text>();
        if (string.IsNullOrEmpty(key))
            key = label.text;
    }

    void OnEnable()
    {
        Refresh();
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged += Refresh;
    }

    void OnDisable()
    {
        if (LocalizationManager.Instance != null)
            LocalizationManager.Instance.OnLanguageChanged -= Refresh;
    }

    public void Refresh()
    {
        if (label == null) label = GetComponent<TMP_Text>();
        label.text = LocalizationManager.Tr(key);
    }
}
