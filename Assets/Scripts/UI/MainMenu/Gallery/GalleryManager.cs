using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GalleryManager : MonoBehaviour
{
    [Header("Data")]
    public TowerData[] towers;
    public EnemyData[] enemies;

    [Header("Full View")]
    public GameObject fullViewPanel;
    public GameObject galleryPanel;
    public Button fullViewButton;
    public Button closeFullViewButton;

    [Header("Notification")]
    public Sprite badgeSprite;
    public GameObject notificationBadge;
    public GameObject infoButtonBadge;

    [Header("UI - Tabs")]
    public Button towersTabButton;
    public Button enemiesTabButton;
    public GameObject towerListRoot;
    public GameObject enemyListRoot;

    [Header("UI - Listas (Content de un ScrollView)")]
    public Transform towerListContent;
    public Transform enemyListContent;
    public GameObject listButtonPrefab;

    [Header("Level Selector")]
    public Button prevLevelButton;
    public Button nextLevelButton;
    public TMP_Text levelText;

    private TowerData currentTowerData;
    private int currentTowerLevel = 0;

    [Header("Visor 3D")]
    public Camera viewerCamera;
    public Transform modelAnchor;
    public float modelScaleMultiplier = 1f;
    public string viewerLayerName = "ModelViewer";

    [Header("Visor 2D (enemigos)")]
    public GameObject viewer3D;
    public GameObject viewer2D;
    public Image enemyImage;

    [Header("Textos de info")]
    public TMP_Text nameText;
    public TMP_Text hpText;
    public TMP_Text damageText;
    public TMP_Text speedText;
    public TMP_Text descriptionText;

    private GameObject currentModelInstance;
    private Dictionary<EnemyData, GameObject> enemyBadges = new Dictionary<EnemyData, GameObject>();

    void Start()
    {
        RefreshNotification();

        fullViewPanel.SetActive(false);
        fullViewButton.onClick.AddListener(() =>
        {
            fullViewPanel.SetActive(true);
            galleryPanel.SetActive(false);
        });

        closeFullViewButton.onClick.AddListener(() =>
        {
            fullViewPanel.SetActive(false);
            galleryPanel.SetActive(true);
        });

        prevLevelButton.onClick.AddListener(() => ChangeTowerLevel(-1));
        nextLevelButton.onClick.AddListener(() => ChangeTowerLevel(1));

        BuildTowerButtons();
        BuildEnemyButtons();

        if (towersTabButton != null) towersTabButton.onClick.AddListener(ShowTowers);
        if (enemiesTabButton != null) enemiesTabButton.onClick.AddListener(ShowEnemies);

        ShowTowers();
    }

    public void ShowTowers()
    {
        prevLevelButton.gameObject.SetActive(true);
        nextLevelButton.gameObject.SetActive(true);
        levelText.gameObject.SetActive(true);
        viewer3D.SetActive(true);
        viewer2D.SetActive(false);

        if (towerListRoot != null) towerListRoot.SetActive(true);
        if (enemyListRoot != null) enemyListRoot.SetActive(false);

        if (towers != null && towers.Length > 0)
            SelectTower(towers[0]);
    }

    public void ShowEnemies()
    {
        prevLevelButton.gameObject.SetActive(false);
        nextLevelButton.gameObject.SetActive(false);
        levelText.gameObject.SetActive(false);
        viewer3D.SetActive(false);
        viewer2D.SetActive(true);

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
            DestroyImmediate(child.gameObject);

        enemyBadges.Clear();

        foreach (EnemyData data in enemies)
        {
            if (data == null) continue;
            GameObject go = Instantiate(listButtonPrefab, enemyListContent);

            bool discovered = PlayerPrefs.GetInt($"enemy_discovered_{data.enemyName}", 0) == 1;

            TMP_Text label = go.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = discovered ? data.enemyName : "???";

            Button btn = go.GetComponent<Button>();
            EnemyData captured = data;
            if (btn != null) btn.onClick.AddListener(() => SelectEnemy(captured));

            GameObject badge = new GameObject("Badge");
            badge.transform.SetParent(go.transform, false);
            Image badgeImage = badge.AddComponent<Image>();
            badgeImage.sprite = badgeSprite;
            RectTransform badgeRect = badge.GetComponent<RectTransform>();
            badgeRect.anchorMin = new Vector2(1f, 1f);
            badgeRect.anchorMax = new Vector2(1f, 1f);
            badgeRect.anchoredPosition = new Vector2(-10f, -10f);
            badgeRect.sizeDelta = new Vector2(20f, 20f);

            enemyBadges[data] = badge;

            enemyBadges[data] = badge;
        }

        RefreshNotification();
    }

    public void SelectTower(TowerData data)
    {
        currentTowerData = data;
        currentTowerLevel = 0;

        viewer3D.SetActive(true);
        viewer2D.SetActive(false);

        if (data == null) return;
        SpawnModel(data.prefab);
        RefreshTowerStats();
    }
    void RefreshTowerStats()
    {
        if (currentTowerData == null) return;
        TowerLevel lvl = currentTowerData.levels[currentTowerLevel];
        levelText.text = $"Level {currentTowerLevel + 1} / {currentTowerData.levels.Length}";

        prevLevelButton.interactable = currentTowerLevel > 0;
        nextLevelButton.interactable = currentTowerLevel < currentTowerData.levels.Length - 1;

        if (currentTowerData.towerType == TowerType.Attack)
        {
            if (hpText != null) hpText.text = $"Range: {lvl.range:F1}";
            if (damageText != null) damageText.text = $"Dmg: {lvl.damage:F0}";
            if (speedText != null) speedText.text = $"Fire rate: {lvl.attackSpeed:F1}/s";
        }
        else if (currentTowerData.towerType == TowerType.Booster)
        {
            if (hpText != null) hpText.text = $"Range: {lvl.range:F1}";
            if (damageText != null) damageText.text = $"Bonus dmg: +{lvl.damageBonus * 100:F0}%";
            if (speedText != null) speedText.text = $"Bonus firerate: +{lvl.attackSpeedBonus * 100:F0}%";
        }
        else if (currentTowerData.towerType == TowerType.Farm)
        {
            if (hpText != null) hpText.text = "-";
            if (damageText != null) damageText.text = "-";
            if (speedText != null) speedText.text = $"Money/Wave: ${lvl.moneyPerWave}";
        }
        if (descriptionText != null) descriptionText.text = currentTowerData.description;
        if (nameText != null) nameText.text = currentTowerData.name;
    }

    void ChangeTowerLevel(int direction)
    {
        currentTowerLevel = Mathf.Clamp(currentTowerLevel + direction, 0, currentTowerData.levels.Length - 1);
        RefreshTowerStats();
    }
    public void SelectEnemy(EnemyData data)
    {
        PlayerPrefs.SetInt($"enemy_seen_{data.enemyName}", 1);
        PlayerPrefs.Save();
        RefreshNotification();
        if (data == null) return;

        viewer3D.SetActive(false);
        viewer2D.SetActive(true);

        bool discovered = PlayerPrefs.GetInt($"enemy_discovered_{data.enemyName}", 0) == 1;

        enemyImage.sprite = data.sprite;
        enemyImage.color = discovered ? Color.white : Color.black;

        if (nameText != null) nameText.text = discovered ? data.enemyName : "???";
        if (hpText != null) hpText.text = discovered ? $"Vida: {data.hp:F0}" : "?";
        if (damageText != null) damageText.text = discovered ? $"Daño al núcleo: {data.hp:F0}" : "?";
        if (speedText != null) speedText.text = discovered ? $"Velocidad: {data.speed:F1}" : "?";
        if (descriptionText != null) descriptionText.text = discovered ? data.description : "???";
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

        foreach (MonoBehaviour mb in go.GetComponentsInChildren<MonoBehaviour>())
            mb.enabled = false;

        foreach (Collider col in go.GetComponentsInChildren<Collider>())
            col.enabled = false;

        foreach (Rigidbody rb in go.GetComponentsInChildren<Rigidbody>())
            rb.isKinematic = true;
    }
    public void RefreshNotification()
    {
        bool hasNew = false;
        Debug.Log($"hasNew: {hasNew}");
        foreach (EnemyData enemy in enemies)
        {
            bool discovered = PlayerPrefs.GetInt($"enemy_discovered_{enemy.enemyName}", 0) == 1;
            bool seen = PlayerPrefs.GetInt($"enemy_seen_{enemy.enemyName}", 0) == 1;
            bool isNew = discovered && !seen;

            if (enemyBadges.ContainsKey(enemy))
                enemyBadges[enemy].SetActive(isNew);

            if (isNew) hasNew = true;
        }

        if (notificationBadge != null) notificationBadge.SetActive(hasNew);
        if (infoButtonBadge != null) infoButtonBadge.SetActive(hasNew);
    }
    void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}
