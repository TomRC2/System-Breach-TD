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

        // Anillo plano en el suelo (mas legible que la esfera transparente)
        rangeSphere = new GameObject("RangeIndicator");
        rangeSphere.transform.position = owner.position;
        LineRenderer lr = rangeSphere.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.material = FXUtil.SharedSpriteMaterial;
        lr.startWidth = lr.endWidth = 0.08f;
        lr.startColor = lr.endColor = new Color(0.4f, 0.9f, 1f, 0.9f);
        SetRingRadius(lr, range);

        rangeSphere.SetActive(range > 0f);

        if (towerController != null)
            TowerInfoPanel.Instance.Show(towerController);
        else if (boosterTower != null)
            TowerInfoPanel.Instance.ShowBooster(boosterTower);
        else if (farmTower != null)
            TowerInfoPanel.Instance.ShowFarm(farmTower);
    }

    // Deselecciona la torre actualmente seleccionada (si la hay), desde cualquier script
    public static void DeselectCurrent()
    {
        if (currentSelected != null)
            currentSelected.Deselect();
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

        float range;
        if (towerController != null)
            range = towerController.GetEffectiveStats().range;
        else if (boosterTower != null)
            range = boosterTower.GetCurrentLevelStats().range;
        else
            return; // las farms no tienen rango

        LineRenderer lr = rangeSphere.GetComponent<LineRenderer>();
        if (lr != null) SetRingRadius(lr, range);
    }

    // Genera los puntos del circulo a la altura del suelo
    static void SetRingRadius(LineRenderer lr, float radius)
    {
        int segments = 64;
        lr.positionCount = segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            lr.SetPosition(i, new Vector3(Mathf.Cos(angle) * radius, 0.05f, Mathf.Sin(angle) * radius));
        }
    }
}
