using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum FocusMode { Closest, Farthest, MostHP, LeastHP, Fastest, First }

public class TowerController : MonoBehaviour
{
    private TowerData data;
    private int currentLevel = 0; // �ndice del array, nivel 1 = �ndice 0

    public FocusMode focusMode = FocusMode.First;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public TowerLevel GetCurrentStats() => CurrentStats();
    private TowerLevel activeBoost = null;

    private float attackCooldown = 0f;

    public void Initialize(TowerData towerData)
    {
        data = towerData;
        currentLevel = 0;
    }

    public TowerData GetData() => data;
    public int GetCurrentLevel() => currentLevel + 1;
    public bool CanUpgrade() => currentLevel < data.levels.Length - 1;
    public int GetUpgradeCost() => CanUpgrade() ? data.levels[currentLevel].upgradeCost : 0;
    public bool IsBoosted() => activeBoost != null;
    public TowerLevel GetActiveBoost() => activeBoost;
    public void Upgrade()
    {
        if (!CanUpgrade()) return;
        currentLevel++;
    }

    TowerLevel CurrentStats() => data.levels[currentLevel];

    void Update()
    {
        if (data == null || data.levels.Length == 0) return;

        attackCooldown -= Time.deltaTime;

        if (attackCooldown <= 0f)
        {
            GameObject target = GetTarget();
            if (target != null)
            {
                Shoot(target);
                attackCooldown = 1f / GetEffectiveStats().attackSpeed;
            }
        }
    }

    GameObject GetTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, GetEffectiveStats().range);
        List<GameObject> enemies = hits
            .Where(h => h.CompareTag("Enemy"))
            .Select(h => h.gameObject)
            .ToList();

        if (enemies.Count == 0) return null;

        return focusMode switch
        {
            FocusMode.Closest => enemies.OrderBy(e =>
                Vector3.Distance(transform.position, e.transform.position)).First(),
            FocusMode.Farthest => enemies.OrderByDescending(e =>
                Vector3.Distance(transform.position, e.transform.position)).First(),
            FocusMode.MostHP => enemies.OrderByDescending(e =>
                e.GetComponent<EnemyHealth>().GetCurrentHP()).First(),
            FocusMode.LeastHP => enemies.OrderBy(e =>
                e.GetComponent<EnemyHealth>().GetCurrentHP()).First(),
            FocusMode.Fastest => enemies.OrderByDescending(e =>
                e.GetComponent<EnemyMovement>().speed).First(),
            FocusMode.First => enemies.OrderByDescending(e =>
                e.GetComponent<EnemyMovement>().CurrentWaypointIndex())
                .ThenBy(e => Vector3.Distance(
                e.transform.position,
                e.GetComponent<EnemyMovement>().GetCurrentWaypoint().position))
                .First(),
            _ => enemies[0]
        };
    }

    void Shoot(GameObject target)
    {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.GetComponent<Projectile>().Initialize(target, CalculateDamage());

        if (data.attackSFX != null)
            AudioManager.Instance?.PlaySFX(data.attackSFX);
    }

    float CalculateDamage()
    {
        bool isCrit = Random.value < GetEffectiveStats().critChance;
        return isCrit
            ? GetEffectiveStats().damage * data.critMultiplier
            : GetEffectiveStats().damage;
    }
    public TowerLevel GetEffectiveStats()
    {
        TowerLevel base_ = CurrentStats();
        if (activeBoost == null) return base_;

        return new TowerLevel
        {
            damage = base_.damage * (1f + activeBoost.damageBonus),
            attackSpeed = base_.attackSpeed * (1f + activeBoost.attackSpeedBonus),
            range = base_.range * (1f + activeBoost.rangeBonus),
            critChance = base_.critChance,
            upgradeCost = base_.upgradeCost
        };
    }

    public void ApplyBoost(TowerLevel boost)
    {
        activeBoost = boost;
    }

    public void RemoveBoost()
    {
        activeBoost = null;
    }

    void OnDrawGizmosSelected()
    {
        if (data == null || data.levels.Length == 0) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, GetEffectiveStats().range);
    }
}