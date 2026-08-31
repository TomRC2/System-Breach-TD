using System.Collections;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    [Header("Referencias")]
    public TMP_Text text;

    [Header("Normal")]
    public float duration = 1f;
    public float riseHeight = 1.5f;   // cuánto sube en Z
    public Color normalColor = Color.red;

    [Header("Crítico")]
    public Color critColor = new Color(0.6f, 0f, 1f); // violeta
    public float critScaleMin = 0.8f;
    public float critScaleMax = 1.4f;
    public float critPulseSpeed = 8f;

    private bool isCrit = false;
    private CanvasGroup cg;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();
    }

    public void Setup(float damage, bool crit, Vector3 worldPosition)
    {
        if (!OptionsManager.IsDamageTextEnabled())
        {
            Release();
            return;
        }

        isCrit = crit;
        transform.position = worldPosition;
        transform.rotation = Camera.main.transform.rotation;
        // Pooling: una instancia reciclada puede llegar con la escala/alpha
        // en el estado en que quedo al terminar su animacion anterior.
        transform.localScale = Vector3.one;
        cg.alpha = 1f;
        text.text = crit ? $"<b>{Mathf.RoundToInt(damage)}!</b>" : Mathf.RoundToInt(damage).ToString();
        text.color = crit ? critColor : normalColor;

        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float height = riseHeight * (1f - (2f * t - 1f) * (2f * t - 1f));
            transform.position = startPos + new Vector3(0f, 0f, +height);

            cg.alpha = t < 0.5f ? 1f : 1f - ((t - 0.5f) / 0.5f);

            if (isCrit)
            {
                float scale = Mathf.Lerp(critScaleMin, critScaleMax,
                    (Mathf.Sin(elapsed * critPulseSpeed) + 1f) / 2f);
                transform.localScale = Vector3.one * scale;
            }

            yield return null;
        }

        Release();
    }

    // Vuelve al pool si vino de ObjectPooler.Get(); si no, se destruye normalmente
    // (compatibilidad con instancias creadas a mano / en tests).
    void Release()
    {
        if (ObjectPooler.Instance != null)
            ObjectPooler.Instance.Release(gameObject);
        else
            Destroy(gameObject);
    }
}
