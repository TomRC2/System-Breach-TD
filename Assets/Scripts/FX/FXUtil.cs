using UnityEngine;

// Utilidades visuales generadas por codigo (sin necesidad de assets nuevos)
public static class FXUtil
{
    private static Sprite circleSprite;
    private static Material spriteMat;

    // Material compartido para lineas y flashes (evita fugas de materiales)
    public static Material SharedSpriteMaterial
    {
        get
        {
            if (spriteMat == null)
            {
                Shader shader = Shader.Find("Sprites/Default");
                spriteMat = new Material(shader != null ? shader : Shader.Find("Universal Render Pipeline/Unlit"));
            }
            return spriteMat;
        }
    }

    // Circulo blanco con borde suave, generado una sola vez
    public static Sprite GetCircleSprite()
    {
        if (circleSprite != null) return circleSprite;

        int size = 64;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float half = size / 2f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(half, half)) / half;
                float alpha = Mathf.Clamp01(1f - dist) ;
                alpha = alpha * alpha * (3f - 2f * alpha); // suavizado
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }
        tex.Apply();
        circleSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        return circleSprite;
    }

    // Destello circular que crece y se desvanece (para impactos)
    public static void SpawnImpactFlash(Vector3 position, Color color, float size = 0.5f, float duration = 0.15f)
    {
        GameObject go = new GameObject("ImpactFlash");
        go.transform.position = position;
        if (Camera.main != null)
            go.transform.rotation = Camera.main.transform.rotation;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = GetCircleSprite();
        sr.color = color;
        sr.sortingOrder = 100;

        ImpactFlashAnim anim = go.AddComponent<ImpactFlashAnim>();
        anim.size = size;
        anim.duration = duration;
    }
}

// Animador interno del destello: crece y se desvanece, luego se destruye
public class ImpactFlashAnim : MonoBehaviour
{
    public float size = 0.5f;
    public float duration = 0.15f;

    private float elapsed;
    private SpriteRenderer sr;

    void Awake() => sr = GetComponent<SpriteRenderer>();

    void Update()
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);

        transform.localScale = Vector3.one * Mathf.Lerp(size * 0.3f, size, t);
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 1f - t;
            sr.color = c;
        }

        if (t >= 1f) Destroy(gameObject);
    }
}
