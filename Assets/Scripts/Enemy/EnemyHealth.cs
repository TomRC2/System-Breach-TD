using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EnemyHealth : MonoBehaviour
{
    // Registro estatico de enemigos vivos: las torres lo usan para buscar objetivos
    // sin necesidad de Physics.OverlapSphere ni GetComponent cada frame.
    public static readonly List<EnemyHealth> Active = new List<EnemyHealth>();

    public float maxHP = 100f;
    public float reward = 10f;
    public bool isBoss = false;
    public string displayName = "";

    private float currentHP;
    private bool isDead = false;

    [Header("Combat Visuals")]
    public EnemyHealthBar healthBar;
    public GameObject damageTextPrefab;

    [Header("Game Feel")]
    [Tooltip("Duracion del encogimiento al morir")]
    public float deathShrinkDuration = 0.15f;

    public Action OnDeath;
    public Action OnReach;

    private static ComputerHealth cachedComputer;
    private EnemyVisualFX visualFX;

    private float damageTextHeight = 0.5f;

    // Pooling: escala original del prefab, para restaurarla despues del
    // encogimiento de la animacion de muerte cuando el objeto se recicla.
    private Vector3 baseScale;

    void Awake()
    {
        // Feedback visual (flash al recibir danio, tinte al estar ralentizado)
        visualFX = gameObject.AddComponent<EnemyVisualFX>();

        baseScale = transform.localScale;

        // Altura del texto de danio segun el tamanio real del sprite,
        // para que no quede tapado en enemigos grandes (p. ej. el Tank)
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            // usar la dimension mayor por si el sprite esta rotado hacia la camara
            float spriteHeight = Mathf.Max(sr.bounds.size.y, sr.bounds.size.z);
            damageTextHeight = Mathf.Max(0.5f, spriteHeight * 0.75f);
        }
    }

    void OnEnable()
    {
        if (!Active.Contains(this)) Active.Add(this);
    }

    void OnDisable()
    {
        Active.Remove(this);
    }

    void Start()
    {
        currentHP = maxHP;
        if (healthBar != null)
            healthBar.Setup(transform, maxHP);
    }

    public void Initialize()
    {
        currentHP = maxHP;
        isDead = false;

        // Pooling: sacarse de encima los suscriptores del ciclo de vida anterior
        // antes de que el WaveSpawner se vuelva a suscribir a este mismo enemigo
        // reciclado (si no, un enemigo reusado dispara el callback de muerte
        // varias veces, una por cada vida anterior que tuvo).
        OnDeath = null;
        OnReach = null;

        // Deshacer lo que DeathEffect() dejo apagado/encogido la vez anterior
        transform.localScale = baseScale;

        EnemyMovement mv = GetComponent<EnemyMovement>();
        if (mv != null)
        {
            mv.enabled = true;
            mv.ResetForReuse();
        }

        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = true;

        visualFX?.ResetVisual();

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(true);
            healthBar.Setup(transform, maxHP);
        }
    }

    public bool IsDead() => isDead;
    public float GetCurrentHP() => currentHP;

    public void TakeDamage(float amount, bool isCrit = false)
    {
        if (isDead) return;
        currentHP -= amount;

        visualFX?.Flash(isCrit ? 0.15f : 0.08f);

        if (healthBar != null)
            healthBar.UpdateHP(currentHP);

        if (damageTextPrefab != null && OptionsManager.IsDamageTextEnabled())
        {
            Vector3 pos = transform.position + Vector3.up * damageTextHeight;
            GameObject go = ObjectPooler.Instance != null
                ? ObjectPooler.Instance.Get(damageTextPrefab, pos, Quaternion.identity)
                : Instantiate(damageTextPrefab, pos, Quaternion.identity);
            go.GetComponent<DamageText>().Setup(amount, isCrit, pos);
        }

        if (currentHP <= 0) Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        Active.Remove(this); // las torres dejan de apuntarle de inmediato

        OnDeath?.Invoke();

        if (EconomyManager.Instance != null)
            EconomyManager.Instance.Earn((int)reward);

        if (isBoss)
            AchievementManager.Instance?.RegisterBossKill();

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.RegisterKill(reward, isBoss);

        // Solo escribir en PlayerPrefs la primera vez que se descubre este enemigo
        string discoveredKey = $"enemy_discovered_{displayName}";
        if (PlayerPrefs.GetInt(discoveredKey, 0) == 0)
        {
            PlayerPrefs.SetInt(discoveredKey, 1);
            PlayerPrefs.Save();
        }

        AchievementManager.Instance?.RegisterKill();
        AchievementManager.Instance?.RegisterFirstBlood();

        StartCoroutine(DeathEffect());
    }

    IEnumerator DeathEffect()
    {
        // desactivar logica para que no siga interactuando mientras se encoge
        EnemyMovement mv = GetComponent<EnemyMovement>();
        if (mv != null) mv.enabled = false;
        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = false;
        if (healthBar != null)
            healthBar.gameObject.SetActive(false);

        Vector3 startScale = transform.localScale;
        float elapsed = 0f;
        while (elapsed < deathShrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / deathShrinkDuration);
            transform.localScale = startScale * (1f - t);
            yield return null;
        }

        Release();
    }

    public void ReachComputer()
    {
        if (isDead) return;
        isDead = true;
        Active.Remove(this);

        if (cachedComputer == null)
            cachedComputer = FindFirstObjectByType<ComputerHealth>();
        if (cachedComputer != null)
            cachedComputer.TakeDamage(maxHP);

        OnReach?.Invoke();
        Release();
    }

    // Vuelve al pool si el objeto vino de ObjectPooler.Get(); si no, se destruye
    // normalmente (por compatibilidad con instancias creadas a mano / en tests).
    void Release()
    {
        if (ObjectPooler.Instance != null)
            ObjectPooler.Instance.Release(gameObject);
        else
            Destroy(gameObject);
    }
}
