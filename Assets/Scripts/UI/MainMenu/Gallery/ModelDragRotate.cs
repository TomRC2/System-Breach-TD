using UnityEngine;
using UnityEngine.EventSystems;

// Permite rotar el modelo (horizontal y vertical) arrastrando sobre el RawImage
// del visor, y hacer zoom con la rueda del mouse (o pellizco en mobile, si se
// enruta scrollDelta desde touch).
// Colgar en el mismo RawImage que muestra la RenderTexture de la cámara del visor.
public class ModelDragRotate : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
{
    public ModelTurntable turntable;
    public float resumeAutoRotateDelay = 3f;

    private float resumeTimer;

    void Update()
    {
        if (turntable == null || turntable.autoRotate) return;

        resumeTimer -= Time.deltaTime;
        if (resumeTimer <= 0f)
            turntable.autoRotate = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (turntable != null) turntable.autoRotate = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (turntable == null) return;
        turntable.Drag(eventData.delta.x, eventData.delta.y);
        resumeTimer = resumeAutoRotateDelay;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        resumeTimer = resumeAutoRotateDelay;
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (turntable == null) return;
        turntable.Zoom(eventData.scrollDelta.y);
    }
}
