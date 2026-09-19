using UnityEngine;

// Limita el tamanio MAXIMO (en unidades de canvas) que puede ocupar este
// RectTransform. Si el area disponible del padre es mas chica que ese
// maximo, este objeto se achica proporcionalmente para no desbordarse;
// si el padre es mas grande (pantallas/tablets grandes), se queda fijo
// en el tamanio maximo en vez de seguir creciendo.
//
// Uso: poner este componente en el contenedor raiz del HUD (o de la
// parte del HUD que querés limitar), con maxWidth/maxHeight iguales al
// tamanio de referencia con el que lo disenaste (por ejemplo 1920x1080,
// o el tamanio especifico de esa "area" si es mas chica que la pantalla).
[RequireComponent(typeof(RectTransform))]
public class UIMaxSize : MonoBehaviour
{
    [Tooltip("Ancho maximo (en unidades de canvas) que puede llegar a ocupar")]
    public float maxWidth = 1920f;
    [Tooltip("Alto maximo (en unidades de canvas) que puede llegar a ocupar")]
    public float maxHeight = 1080f;

    private RectTransform rect;
    private RectTransform parentRect;
    private Vector2 lastParentSize;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        parentRect = transform.parent as RectTransform;
        Apply();
    }

    void Update()
    {
        if (parentRect == null) return;
        // Solo recalcular si el area del padre realmente cambio (rotacion,
        // resize de ventana en el Editor, etc.) en vez de escalar cada frame.
        if (parentRect.rect.size != lastParentSize)
            Apply();
    }

    void Apply()
    {
        if (parentRect == null) return;
        lastParentSize = parentRect.rect.size;

        float scaleX = lastParentSize.x / maxWidth;
        float scaleY = lastParentSize.y / maxHeight;
        // Nunca mas grande que 1 (o sea, nunca mas grande que el area maxima),
        // pero se achica si el espacio real disponible es menor.
        float scale = Mathf.Min(1f, scaleX, scaleY);

        rect.localScale = Vector3.one * scale;
    }
}
