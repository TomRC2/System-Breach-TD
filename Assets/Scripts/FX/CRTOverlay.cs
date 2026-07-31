using UnityEngine;
using UnityEngine.UI;

// Overlay retro CRT: scanlines + vinieta, generado por codigo.
// Persiste entre escenas. Se puede desactivar con CRTOverlay.SetEnabled(false)
// (guardado en PlayerPrefs "crt_enabled").
public class CRTOverlay : MonoBehaviour
{
    private const string KEY = "crt_enabled";
    private static CRTOverlay instance;

    private RawImage scanlines;
    private int lastHeight;

    public static bool IsEnabled() => PlayerPrefs.GetInt(KEY, 1) == 1;

    public static void SetEnabled(bool enabled)
    {
        PlayerPrefs.SetInt(KEY, enabled ? 1 : 0);
        PlayerPrefs.Save();
        if (instance != null) instance.gameObject.SetActive(enabled);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        if (instance != null) return;

        GameObject go = new GameObject("CRTOverlay");
        Object.DontDestroyOnLoad(go);

        Canvas canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 30000; // por encima de todo

        instance = go.AddComponent<CRTOverlay>();
        instance.Build();
        go.SetActive(IsEnabled());
    }

    void Build()
    {
        // --- Scanlines: textura de 1x3 repetida verticalmente ---
        Texture2D lineTex = new Texture2D(1, 3, TextureFormat.RGBA32, false);
        lineTex.SetPixel(0, 0, new Color(0f, 0f, 0f, 0f));
        lineTex.SetPixel(0, 1, new Color(0f, 0f, 0f, 0f));
        lineTex.SetPixel(0, 2, new Color(0f, 0f, 0f, 0.16f));
        lineTex.filterMode = FilterMode.Point;
        lineTex.wrapMode = TextureWrapMode.Repeat;
        lineTex.Apply();

        GameObject lineGo = new GameObject("Scanlines");
        lineGo.transform.SetParent(transform, false);
        scanlines = lineGo.AddComponent<RawImage>();
        scanlines.texture = lineTex;
        scanlines.raycastTarget = false;
        Stretch(lineGo);
        UpdateTiling();

        // --- Vinieta: textura radial ---
        int size = 128;
        Texture2D vigTex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float half = size / 2f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(half, half)) / half;
                float a = Mathf.Clamp01(dist - 0.55f) * 0.55f; // solo bordes, suave
                vigTex.SetPixel(x, y, new Color(0f, 0f, 0f, a * a * 1.4f));
            }
        }
        vigTex.Apply();

        GameObject vigGo = new GameObject("Vignette");
        vigGo.transform.SetParent(transform, false);
        RawImage vig = vigGo.AddComponent<RawImage>();
        vig.texture = vigTex;
        vig.raycastTarget = false;
        Stretch(vigGo);
    }

    static void Stretch(GameObject go)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    void UpdateTiling()
    {
        lastHeight = Screen.height;
        // una scanline cada 3 pixeles de pantalla
        scanlines.uvRect = new Rect(0f, 0f, 1f, Screen.height / 3f);
    }

    void Update()
    {
        if (Screen.height != lastHeight) UpdateTiling();
    }
}
