using UnityEngine;

// Rota (horizontal y vertical) y hace zoom del modelo 3D mostrado en el visor.
// Colgar en el mismo objeto usado como "modelAnchor" en GalleryManager.
public class ModelTurntable : MonoBehaviour
{
    [Header("Rotación automática")]
    public float rotationSpeed = 30f;
    public bool autoRotate = true;

    [Header("Rotación vertical (pitch)")]
    public float minPitch = -60f;
    public float maxPitch = 60f;
    public float dragSensitivity = 0.5f;

    [Header("Zoom")]
    public float zoomSpeed = 0.1f;
    public float minScale = 0.5f;
    public float maxScale = 3f;

    private float yaw;
    private float pitch;
    private float currentScale = 1f;

    void Start()
    {
        Vector3 e = transform.eulerAngles;
        yaw = e.y;
        pitch = e.x > 180f ? e.x - 360f : e.x;
        currentScale = transform.localScale.x;
    }

    void Update()
    {
        if (autoRotate)
        {
            yaw += rotationSpeed * Time.deltaTime;
            ApplyRotation();
        }
    }

    // deltaX mueve el yaw (horizontal), deltaY mueve el pitch (vertical)
    public void Drag(float deltaX, float deltaY)
    {
        autoRotate = false;
        yaw -= deltaX * dragSensitivity;
        pitch = Mathf.Clamp(pitch - deltaY * dragSensitivity, minPitch, maxPitch);
        ApplyRotation();
    }

    public void Zoom(float scrollDelta)
    {
        currentScale = Mathf.Clamp(currentScale + scrollDelta * zoomSpeed, minScale, maxScale);
        transform.localScale = Vector3.one * currentScale;
    }

    void ApplyRotation()
    {
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
