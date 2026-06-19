using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePanel : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [Tooltip("El panel completo a mover. Si se deja vacío, se mueve este mismo GameObject.")]
    public RectTransform targetPanel;

    private RectTransform rect;
    private RectTransform canvasRect;
    private Vector2 dragOffset;

    void Awake()
    {
        rect = targetPanel != null ? targetPanel : GetComponent<RectTransform>();
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

        dragOffset = (Vector2)rect.localPosition - localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

        Vector2 newPos = localPoint + dragOffset;

        Vector2 canvasSize = canvasRect.rect.size;
        Vector2 panelSize = rect.rect.size;

        float minX = -canvasSize.x / 2f + panelSize.x / 2f;
        float maxX = canvasSize.x / 2f - panelSize.x / 2f;
        float minY = -canvasSize.y / 2f + panelSize.y / 2f;
        float maxY = canvasSize.y / 2f - panelSize.y / 2f;

        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);

        rect.localPosition = newPos;
    }
}