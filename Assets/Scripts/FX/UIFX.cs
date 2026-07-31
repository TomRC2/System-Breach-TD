using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// ---------- Transicion de paneles (fade + escala) ----------
public static class PanelFX
{
    // Reemplazo de panel.SetActive(true): anima solo si estaba cerrado
    public static void Show(GameObject panel)
    {
        if (panel == null) return;
        if (panel.activeSelf) return;

        panel.SetActive(true);
        PanelFXAnim anim = panel.GetComponent<PanelFXAnim>();
        if (anim == null) anim = panel.AddComponent<PanelFXAnim>();
        anim.Play();
    }
}

public class PanelFXAnim : MonoBehaviour
{
    private CanvasGroup cg;
    private Vector3 baseScale;
    private float t;
    private bool playing;
    private const float DURATION = 0.15f;

    void Awake()
    {
        baseScale = transform.localScale;
        cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();
    }

    public void Play()
    {
        t = 0f;
        playing = true;
        Apply(0f);
    }

    void Update()
    {
        if (!playing) return;
        t += Time.unscaledDeltaTime / DURATION;
        if (t >= 1f) { t = 1f; playing = false; }
        Apply(t);
    }

    void Apply(float k)
    {
        cg.alpha = k;
        transform.localScale = baseScale * (0.95f + 0.05f * k);
    }

    void OnDisable()
    {
        playing = false;
        if (cg != null) cg.alpha = 1f;
        transform.localScale = baseScale;
    }
}

// ---------- Hover/press en botones (+ pulso opcional) ----------
public class ButtonHoverFX : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Tooltip("Pulso suave continuo (p. ej. para resaltar el siguiente nivel)")]
    public bool idlePulse = false;

    private Vector3 baseScale;
    private bool hovered, pressed;
    private Selectable selectable;

    void Awake()
    {
        baseScale = transform.localScale;
        selectable = GetComponent<Selectable>();
    }

    void OnDisable()
    {
        hovered = pressed = false;
        transform.localScale = baseScale;
    }

    public void OnPointerEnter(PointerEventData e) => hovered = true;
    public void OnPointerExit(PointerEventData e) { hovered = false; pressed = false; }
    public void OnPointerDown(PointerEventData e) => pressed = true;
    public void OnPointerUp(PointerEventData e) => pressed = false;

    void Update()
    {
        bool interactable = selectable == null || selectable.interactable;

        float target = 1f;
        if (interactable && pressed) target = 0.94f;
        else if (interactable && hovered) target = 1.06f;
        else if (idlePulse && interactable)
            target = 1f + 0.04f * Mathf.Sin(Time.unscaledTime * 4f);

        float k = 1f - Mathf.Exp(-14f * Time.unscaledDeltaTime);
        transform.localScale = Vector3.Lerp(transform.localScale, baseScale * target, k);
    }
}

// ---------- Bootstrap: aniade ButtonHoverFX a todos los botones automaticamente ----------
public class UIFXBootstrap : MonoBehaviour
{
    private float timer;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        GameObject go = new GameObject("UIFXBootstrap");
        Object.DontDestroyOnLoad(go);
        go.AddComponent<UIFXBootstrap>();
    }

    void Update()
    {
        timer -= Time.unscaledDeltaTime;
        if (timer > 0f) return;
        timer = 1f; // escanear una vez por segundo (cubre botones creados en runtime)

        foreach (Button b in Object.FindObjectsByType<Button>(
            FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (b.GetComponent<ButtonHoverFX>() == null)
                b.gameObject.AddComponent<ButtonHoverFX>();
        }
    }
}
