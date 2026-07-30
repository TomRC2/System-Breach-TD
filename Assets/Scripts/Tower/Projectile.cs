using UnityEngine;

public class Projectile : MonoBehaviour
{
    private EnemyHealth target;
    private float damage;
    private bool isCrit;
    private float splashRadius;
    private float slowAmount;
    private float slowDuration;

    public float speed = 15f;
    [Tooltip("Fraccion del danio que reciben los enemigos alcanzados por el area (splash)")]
    [Range(0f, 1f)] public float splashDamageFactor = 0.5f;
    [Tooltip("Tiempo de vida maximo por seguridad")]
    public float maxLifetime = 5f;

    private float lifetime = 0f;
    private Color fxColor = new Color(0.4f, 0.9f, 1f);

    void Start()
    {
        // Tomar el color del proyectil para la estela y el destello
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) fxColor = sr.color;
        else
        {
            Renderer rend = GetComponentInChildren<Renderer>();
            if (rend != null && rend.sharedMaterial != null && rend.sharedMaterial.HasProperty("_BaseColor"))
                fxColor = rend.sharedMaterial.GetColor("_BaseColor");
        }

        // Estela generada por codigo (si el prefab no trae una propia)
        if (GetComponentInChildren<TrailRenderer>() == null)
        {
            TrailRenderer trail = gameObject.AddComponent<TrailRenderer>();
            trail.time = 0.15f;
            trail.startWidth = 0.12f;
            trail.endWidth = 0f;
            trail.material = FXUtil.SharedSpriteMaterial;
            trail.startColor = fxColor;
            Color end = fxColor; end.a = 0f;
            trail.endColor = end;
        }
    }

    // Compatibilidad con la firma anterior
    public void Initialize(GameObject targetGo, float dmg)
    {
        EnemyHealth hp = targetGo != null ? targetGo.GetComponent<EnemyHealth>() : null;
        Initialize(hp, dmg, false, 0f, 0f, 0f);
    }

    public void Initialize(EnemyHealth targetEnemy, float dmg, bool crit,
        float splash, float slowAmt, float slowDur)
    {
        target = targetEnemy;
        damage = dmg;
        isCrit = crit;
        splashRadius = splash;
        slowAmount = slowAmt;
        slowDuration = slowDur;
    }

    void Update()
    {
        lifetime += Time.deltaTime;
        if (target == null || lifetime > maxLifetime)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPos = target.transform.position;
        transform.position = Vector3.MoveTowards(
            transform.position, targetPos, speed * Time.deltaTime);

        // orientar el proyectil hacia donde viaja
        Vector3 dir = targetPos - transform.position;
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir);

        if ((transform.position - targetPos).sqrMagnitude < 0.15f * 0.15f)
            Impact();
    }

    void Impact()
    {
        // Destello de impacto (mas grande si hay danio en area)
        float flashSize = splashRadius > 0f ? Mathf.Max(0.6f, splashRadius) : (isCrit ? 0.7f : 0.45f);
        FXUtil.SpawnImpactFlash(transform.position, fxColor, flashSize, isCrit ? 0.22f : 0.15f);

        if (target != null)
        {
            target.TakeDamage(damage, isCrit);
            ApplySlow(target);
        }

        // Danio en area (solo si la torre tiene splashRadius > 0)
        if (splashRadius > 0f)
        {
            float splashSqr = splashRadius * splashRadius;
            var enemies = EnemyHealth.Active;
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                EnemyHealth enemy = enemies[i];
                if (enemy == null || enemy == target || enemy.IsDead()) continue;
                if ((enemy.transform.position - transform.position).sqrMagnitude > splashSqr) continue;

                enemy.TakeDamage(damage * splashDamageFactor, false);
                ApplySlow(enemy);
            }
        }

        Destroy(gameObject);
    }

    void ApplySlow(EnemyHealth enemy)
    {
        if (slowAmount <= 0f || slowDuration <= 0f || enemy == null) return;
        EnemyMovement mv = enemy.GetComponent<EnemyMovement>();
        if (mv != null) mv.ApplySlow(slowAmount, slowDuration);
    }
}
