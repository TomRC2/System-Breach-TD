using UnityEngine;
using UnityEngine.EventSystems;

public class TowerClickHandler : MonoBehaviour, IPointerClickHandler
{
    private TowerController towerController;
    private BoosterTower boosterTower;
    private FarmTower farmTower;
    private Renderer[] renderers;

    [Header("Highlight")]
    public Color highlightColor = new Color(0.4f, 0.8f, 1f, 1f);

    [Header("Range Sphere")]
    [Tooltip("Asignar un material transparente URP desde el Inspector (evita Shader.Find en builds)")]
    public Material rangeMaterial;

    private MaterialPropertyBlock mpb;
    private GameObject rangeSphere;
    private static TowerClickHandler currentSelected;

    void Start()
    {
        farmTower = GetComponentInParent<FarmTower>();
        towerController = GetComponentInParent<TowerController>();
        boosterTower = GetComponentInParent<BoosterTower>();
        renderers = GetComponentsInParent<Renderer>();
        mpb = new MaterialPropertyBlock();
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

        // Usar MaterialPropertyBlock en lugar de .material para evitar instancias huerfanas
        mpb.SetColor("_BaseColor", highlightColor);
        foreach (Renderer rend in renderers)
            rend.SetPropertyBlock(mpb);

        float range = 0f;
        Transform owner = transform;

        if (towerController != null)
        {
            range = towerController.GetCurrentStats().range;
            owner = towerController.transform;
        }
        else if (boosterTower != null)
        {
            range = boosterTower.GetCurrentLevelStats().range;
            owner = boosterTower.transform;
        }

        rangeSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rangeSphere.transform.position = owner.position;
        rangeSphere.transform.localScale = Vector3.one * range * 2f;
        Destroy(rangeSphere.GetComponent<Collider>());

        if (rangeMaterial != null)
        {
            // Material asignado desde el Inspector — seguro en builds
            rangeSphere.GetComponent<Renderer>().sharedMaterial = rangeMaterial;
        }
        else
        {
            // Fallback: buscar shader en runtime (puede fallar en builds con shader stripping)
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader != null)
            {
                Material mat = new Material(shader);
                mat.SetFloat("_Surface", 1f);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.renderQueue = 3000;
                mat.color = new Color(0.4f, 0.8f, 1f, 0.15f);
                rangeSphere.GetComponent<Renderer>().material = mat;
            }
            else
            {
                Debug.LogWarning("TowerClickHandler: shader URP/Lit no encontrado. Asigna 'rangeMaterial' en el Inspector.");
            }
        }

        rangeSphere.SetActive(range > 0f);

        if (towerController != null)
            TowerInfoPanel.Instance.Show(towerController);
        else if (boosterTower != null)
            TowerInfoPanel.Instance.ShowBooster(boosterTower);
        else if (farmTower != null)
            TowerInfoPanel.Instance.ShowFarm(farmTower);
    }

    public void Deselect()
    {
        currentSelected = null;

        // Remover el PropertyBlock restaura el material original sin instancias huerfanas
        foreach (Renderer rend in renderers)
            rend.SetPropertyBlock(null);

        if (rangeSphere != null) Destroy(rangeSphere);
        TowerInfoPanel.Instance.Close();
    }

    public void RefreshRange()
    {
        if (rangeSphere == null) return;
        float range = towerController != null
            ? towerController.GetEffectiveStats().range
            : boosterTower.GetCurrentLevelStats().range;
        rangeSphere.transform.localScale = Vector3.one * range * 2f;
    }
}
