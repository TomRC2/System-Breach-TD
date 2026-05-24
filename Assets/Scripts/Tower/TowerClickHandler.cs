using UnityEngine;
using UnityEngine.EventSystems;

public class TowerClickHandler : MonoBehaviour, IPointerClickHandler
{
    private TowerController towerController;
    private Renderer[] renderers;
    private Color[] originalColors;

    [Header("Highlight")]
    public Color highlightColor = new Color(0.4f, 0.8f, 1f, 1f);

    private GameObject rangeSphere;
    private static TowerClickHandler currentSelected;

    void Start()
    {
        towerController = GetComponentInParent<TowerController>();
        renderers = GetComponentsInParent<Renderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].material.color;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentSelected == this)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out RaycastHit hit) || hit.collider.gameObject != gameObject)
                    Deselect();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentSelected != null && currentSelected != this)
            currentSelected.Deselect();

        if (currentSelected == this)
        {
            Deselect();
            return;
        }

        Select();
    }

    void Select()
    {
        currentSelected = this;

        foreach (Renderer rend in renderers)
            rend.material.color = highlightColor;

        float range = towerController.GetCurrentStats().range;

        rangeSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rangeSphere.transform.position = towerController.transform.position;
        rangeSphere.transform.localScale = Vector3.one * range * 2f;
        Destroy(rangeSphere.GetComponent<Collider>());

        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetFloat("_Surface", 1f);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.renderQueue = 3000;
        mat.color = new Color(0.4f, 0.8f, 1f, 0.15f);
        rangeSphere.GetComponent<Renderer>().material = mat;

        TowerInfoPanel.Instance.Show(towerController);
    }

    public void Deselect()
    {
        currentSelected = null;

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = originalColors[i];

        if (rangeSphere != null) Destroy(rangeSphere);
        TowerInfoPanel.Instance.Close();
    }
}