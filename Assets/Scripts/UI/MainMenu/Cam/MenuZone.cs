using UnityEngine;

// Poné este script en cada objeto 3D del entorno que sea clickeable.
// Asigná el destino desde el Inspector.

public class MenuZone : MonoBehaviour
{
    public enum Destination { LevelSelect, Options, Credits }

    [Header("Destino")]
    public Destination destination;

    [Header("Highlight (opcional)")]
    public Renderer zoneRenderer;
    public Color hoverColor = new Color(1f, 1f, 0.3f, 0.5f);
    private Color originalColor;

    void Start()
    {
        if (zoneRenderer != null)
            originalColor = zoneRenderer.material.color;
    }

    void OnMouseDown()
    {
        switch (destination)
        {
            case Destination.LevelSelect:
                CameraMenuController.Instance.GoToLevelSelect();
                break;
            case Destination.Options:
                CameraMenuController.Instance.GoToOptions();
                break;
            case Destination.Credits:
                CameraMenuController.Instance.GoToCredits();
                break;
        }
    }

    void OnMouseEnter()
    {
        if (zoneRenderer != null)
            zoneRenderer.material.color = hoverColor;
    }

    void OnMouseExit()
    {
        if (zoneRenderer != null)
            zoneRenderer.material.color = originalColor;
    }
}
