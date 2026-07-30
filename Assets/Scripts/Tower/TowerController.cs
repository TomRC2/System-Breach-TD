using System.Collections.Generic;
using UnityEngine;

public enum FocusMode { Closest, Farthest, MostHP, LeastHP, Fastest, First }

public class TowerController : MonoBehaviour
{
    private TowerData data;
    private int currentLevel = 0;
    public FocusMode focusMode = FocusMode.First;
    public GameObject projectilePrefab;
    public Transform firePoint;
    private TowerLevel activeBoost = null;

    [Header("References")]
    public Transform rotatingPart;

    [Header("Game Feel")]
    [Tooltip("Velocidad de giro al apuntar (mayor = mas rapido)")]
    public float aimSpeed = 14f;

    private float attackCooldown = 0f;
    private float retargetTimer = 0f;
    private const float RETARGET_INTERVAL = 0.1f; // re-elegir objetivo 10 veces/seg en vez de cada frame
    private EnemyHealth currentTarget;
    private TowerLevel cachedStats; // stats efectivas cacheadas (evita alocar cada frame)

    public TowerLevel GetCurrentStats() => CurrentStats();
    public TowerData GetData() => data;
    public int GetCurrentLevel() => currentLevel + 1;
    public bool CanUpgrade() => data != null && currentLevel < data.levels.Length - 1;
    public int GetUpgradeCost() => CanUpgrade() ? data.levels[currentLevel].upgradeCost : 0;
    public bool IsBoosted() => activeBoost != null;
    public TowerLevel GetActiveBoost() => activeBoost;

    public void Initialize(TowerData towerData)
    {
        data = towerData;
        currentLevel = 0;
        RebuildStats();
    }

    public void Upgrade()
    {
        if (!CanUpgrade()) return;
        currentLevel++;
        RebuildStats();
    }

    TowerLevel CurrentStats() => data.levels[currentLevel];

    public TowerLevel GetEffectiveStats()
    {
        if (cachedStats == null) RebuildStats();
        return cachedStats;
    }

    void RebuildStats()
    {
        if (data == null || data.levels.Length == 0) { cachedStats = null; return; }

        TowerLevel base_ = CurrentStats();
        if (activeBoost == null)
        {
            cachedStats = base_;
            return;
        }

        cachedStats = new TowerLevel
        {
            damage = base_.damage * (1f + activeBoost.damageBonus),
            attackSpeed = base_.attackSpeed * (1f + activeBoost.attackSpeedBonus),
            range = base_.range * (1f + activeBoost.rangeBonus),
            critChance = base_.critChance,
            upgradeCost = base_.upgradeCost,
            splashRadius = base_.splashRadius,
            slowAmount = base_.slowAmount,
            slowDuration = base_.slowDuration,
            moneyPerWave = base_.moneyPerWave
        };
    }

    public void ApplyBoost(TowerLevel boost)
    {
        activeBoost = boost;
        RebuildStats();
    }

    public void RemoveBoost()
    {
        activeBoost = null;
        RebuildStats();
    }

    void Update()
    {
        if (data == null || data.levels.Length == 0) return;

        TowerLevel stats = GetEffectiveStats();
        attackCooldown -= Time.deltaTime;
        retargetTimer -= Time.deltaTime;

        if (retargetTimer <= 0f || !IsValidTarget(currentTarget, stats.range))
        {
            currentTarget = GetTarget(stats.range);
            retargetTimer = RETARGET_INTERVAL;
        }

        if (currentTarget != null)
            AimAt(currentTarget.transform.position);

        if (attackCooldown <= 0f && currentTarget != null)
        {
            Shoot(currentTarget);
            attackCooldown = 1f / stats.attackSpeed;
        }
    }

    bool IsValidTarget(EnemyHealth enemy, float range)
    {
        if (enemy == null || enemy.IsDead()) return false;
        return (enemy.transform.position - transform.position).sqrMagnitude <= range * range;
    }

    // Usa el registro estatico de enemigos (sin Physics.OverlapSphere ni LINQ)
    EnemyHealth GetTarget(float range)
    {
        float rangeSqr = range * range;
        EnemyHealth best = null;
        float bestScore = 0f;

        List<EnemyHealth> enemies = EnemyHealth.Active;
        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyHealth enemy = enemies[i];
            if (enemy == null || enemy.IsDead()) continue;

            float distSqr = (enemy.transform.position - transform.position).sqrMagnitude;
            if (distSqr > rangeSqr) continue;

            float score = ScoreFor(enemy, distSqr);
            if (best == null || score > bestScore)
            {
                best = enemy;
                bestScore = score;
            }
        }
        return best;
    }

    // Mayor puntaje = mejor objetivo segun el modo de foco actual
    float ScoreFor(EnemyHealth enemy, float distSqr)
    {
        switch (focusMode)
        {
            case FocusMode.Closest: return -distSqr;
            case FocusMode.Farthest: return distSqr;
            case FocusMode.MostHP: return enemy.GetCurrentHP();
            case FocusMode.LeastHP: return -enemy.GetCurrentHP();
            case FocusMode.Fastest:
            {
                EnemyMovement mv = enemy.GetComponent<EnemyMovement>();
                return mv != null ? mv.speed : 0f;
            }
            case FocusMode.First:
            default:
            {
                EnemyMovement mv = enemy.GetComponent<EnemyMovement>();
                if (mv == null) return 0f;
                float distToWaypoint = Vector3.Distance(
                    enemy.transform.position, mv.GetCurrentWaypoint().position);
                // prioriza mayor indice de waypoint, desempata por cercania al siguiente
                return mv.CurrentWaypointIndex() * 10000f - distToWaypoint;
            }
        }
    }

    void AimAt(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion look = Quaternion.LookRotation(dir);
        float t = 1f - Mathf.Exp(-aimSpeed * Time.deltaTime); // suavizado independiente del framerate

        if (rotatingPart != null)
        {
            Quaternion targetRot = look * Quaternion.Inverse(rotatingPart.parent.rotation);
            rotatingPart.rotation = Quaternion.Slerp(rotatingPart.rotation, targetRot, t);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, look, t);
        }
    }

    void Shoot(EnemyHealth target)
    {
        TowerLevel stats = GetEffectiveStats();
        bool isCrit = Random.value < stats.critChance;
        float damage = isCrit ? stats.damage * data.critMultiplier : stats.damage;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.GetComponent<Projectile>().Initialize(
            target, damage, isCrit, stats.splashRadius, stats.slowAmount, stats.slowDuration);

        if (data.attackSFX != null)
            AudioManager.Instance?.PlaySFXLimited(data.attackSFX);
    }

    void OnDrawGizmosSelected()
    {
        if (data == null || data.levels.Length == 0) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, GetEffectiveStats().range);
    }
}
