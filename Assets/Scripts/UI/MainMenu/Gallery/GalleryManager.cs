using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Controla el visor de "Info": lista de torres/enemigos + modelo 3D + stats.
// Colgar este script en el GameObject raíz del sub-panel de galería dentro de InfoPanel.
public class GalleryManager : MonoBehaviour
{
    [Header("Data")]
    public TowerData[] towers;
    public EnemyData[] enemies;

    [Header("UI - Tabs")]
    public Button towersTabButton;
    public Button enemiesTabButton;
    public GameObject towerListRoot;
    public GameObject enemyListRoot;

    [Header("UI - Listas (Content de un ScrollView)")]
    public Transform towerListContent;
    public Transform enemyListContent;
    public GameObject listButtonPrefab; // Prefab simple: Button + TMP_Text hijo

    [Header("Visor 3D")]
    public Transform modelAnchor;       // Pivot (hijo de la cámara del visor) donde se instancia el modelo
    public float modelScaleMultiplier = 1f;
    public string viewerLayerName = "ModelViewer";

    [Header("Textos de info")]
    public TMP_Text nameText;
    public TMP_Text hpText;
    public TMP_Text damageText;
    public TMP_Text speedText;
    public TMP_Text descriptionText;

    private GameObject currentModelInstance;

    void Start()
    {
        BuildTowerButtons();
        BuildEnemyButtons();

        if (towersTabButton != null) towersTabButton.onClick.AddListener(ShowTowers);
        if (enemiesTabButton != null) enemiesTabButton.onClick.AddListener(ShowEnemies);

        ShowTowers();
    }

    public void ShowTowers()
    {
        if (towerListRoot != null) towerListRoot.SetActive(true);
        if (enemyListRoot != null) enemyListRoot.SetActive(false);

        if (towers != null && towers.Length > 0)
            SelectTower(towers[0]);
    }

    public void ShowEnemies()
    {
        if (towerListRoot != null) towerListRoot.SetActive(false);
        if (enemyListRoot != null) enemyListRoot.SetActive(true);

        if (enemies != null && enemies.Length > 0)
            SelectEnemy(enemies[0]);
    }

    void BuildTowerButtons()
    {
        if (towerListContent == null || listButtonPrefab == null || towers == null) return;

        foreach (Transform child in towerListContent)
            Destroy(child.gameObject);

        foreach (TowerData data in towers)
        {
            if (data == null) continue;
            GameObject go = Instantiate(listButtonPrefab, towerListContent);
            TMP_Text label = go.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = data.towerName;

            Button btn = go.GetComponent<Button>();
            TowerData captured = data;
            if (btn != null) btn.onClick.AddListener(() => SelectTower(captured));
        }
    }

    void BuildEnemyButtons()
    {
        if (enemyListContent == null || listButtonPrefab == null || enemies == null) return;

        foreach (Transform child in enemyListContent)
            Destroy(child.gameObject);

        foreach (EnemyData data in enemies)
        {
            if (data == null) continue;
            GameObject go = Instantiate(listButtonPrefab, enemyListContent);
            TMP_Text label = go.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = data.enemyName;

            Button btn = go.GetComponent<Button>();
            EnemyData captured = data;
            if (btn != null) btn.onClick.AddListener(() => SelectEnemy(captured));
        }
    }

    public void SelectTower(TowerData data)
    {
        if (data == null) return;
        SpawnModel(data.prefab);

        TowerLevel lvl1 = (data.levels != null && data.levels.Length > 0) ? data.levels[0] : null;

        if (nameText != null) nameText.text = data.towerName;
        if (hpText != null) hpText.text = "Vida: -";
        if (damageText != null) damageText.text = lvl1 != null ? $"Daño: {lvl1.damage:F0}" : "Daño: -";
        if (speedText != null) speedText.text = lvl1 != null ? $"Velocidad de ataque: {lvl1.attackSpeed:F1}/s" : "Velocidad: -";
        if (descriptionText != null) descriptionText.text = data.description;
    }

    public void SelectEnemy(EnemyData data)
    {
        if (data == null) return;
        SpawnModel(data.prefab);

        if (nameText != null) nameText.text = data.enemyName;
        if (hpText != null) hpText.text = $"Vida: {data.hp:F0}";
        if (damageText != null) damageText.text = $"Daño al núcleo: {data.hp:F0}";
        if (speedText != null) speedText.text = $"Velocidad: {data.speed:F1}";
        if (descriptionText != null) descriptionText.text = data.description;
    }

    void SpawnModel(GameObject prefab)
    {
        if (currentModelInstance != null)
        {
            Destroy(currentModelInstance);
            currentModelInstance = null;
        }

        if (prefab == null || modelAnchor == null) return;

        currentModelInstance = Instantiate(prefab, modelAnchor);
        currentModelInstance.transform.localPosition = Vector3.zero;
        currentModelInstance.transform.localRotation = Quaternion.identity;
        currentModelInstance.transform.localScale = Vector3.one * modelScaleMultiplier;

        PrepareForViewer(currentModelInstance);
    }

    void PrepareForViewer(GameObject go)
    {
        int layer = LayerMask.NameToLayer(viewerLayerName);
        if (layer >= 0) SetLayerRecursively(go, layer);

        // Apaga scripts de gameplay (movimiento, IA, disparo, etc.) para que el modelo quede quieto.
        foreach (MonoBehaviour mb in go.GetComponentsInChildren<MonoBehaviour>())
            mb.enabled = false;

        foreach (Collider col in go.GetComponentsInChildren<Collider>())
            col.enabled = false;

        foreach (Rigidbody rb in go.GetComponentsInChildren<Rigidbody>())
            rb.isKinematic = true;
    }

    void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}
