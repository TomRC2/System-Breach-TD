using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Anuncio grande "WAVE X / Y" al comenzar cada oleada. Creado 100% por codigo.
public static class WaveBanner
{
    public static void Show(int current, int total)
    {
        GameObject go = new GameObject("WaveBanner");

        Canvas canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 20000;

        CanvasScaler scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        GameObject textGo = new GameObject("Text");
        textGo.transform.SetParent(go.transform, false);
        TextMeshProUGUI text = textGo.AddComponent<TextMeshProUGUI>();
        text.text = $"WAVE {current} / {total}";
        text.fontSize = 90;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(0.55f, 1f, 0.35f);
        text.raycastTarget = false;

        RectTransform rt = textGo.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.72f);
        rt.anchorMax = new Vector2(0.5f, 0.72f);
        rt.sizeDelta = new Vector2(1200, 160);
        rt.anchoredPosition = Vector2.zero;

        go.AddComponent<WaveBannerAnim>().Setup(text);
    }
}

public class WaveBannerAnim : MonoBehaviour
{
    private TextMeshProUGUI text;

    public void Setup(TextMeshProUGUI t)
    {
        text = t;
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float inDur = 0.2f, hold = 0.9f, outDur = 0.35f;
        Transform tr = text.transform;

        // entrada: encoge desde grande + fade in
        float elapsed = 0f;
        while (elapsed < inDur)
        {
            elapsed += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(elapsed / inDur);
            tr.localScale = Vector3.one * Mathf.Lerp(1.6f, 1f, k * k);
            SetAlpha(k);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(hold);

        // salida: fade out subiendo un poco
        elapsed = 0f;
        Vector2 startPos = ((RectTransform)tr).anchoredPosition;
        while (elapsed < outDur)
        {
            elapsed += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(elapsed / outDur);
            SetAlpha(1f - k);
            ((RectTransform)tr).anchoredPosition = startPos + Vector2.up * (30f * k);
            yield return null;
        }

        Destroy(gameObject);
    }

    void SetAlpha(float a)
    {
        Color c = text.color;
        c.a = a;
        text.color = c;
    }
}
