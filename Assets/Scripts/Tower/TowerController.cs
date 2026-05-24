using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum FocusMode { Closest, Farthest, MostHP, LeastHP, Fastest, First }

public class TowerController : MonoBehaviour
{
    private TowerData data;
    private int currentLevel = 0; // índice del array, nivel 1 = índice 0

    public FocusMode focusMode = FocusMode.First;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public TowerLevel GetCurrentStats() => CurrentStats();

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
                attackCooldown = 1f / CurrentStats().attackSpeed;
            }
        }
    }

    GameObject GetTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, CurrentStats().range);
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
                e.GetComponent<EnemyMovement>().CurrentWaypointIndex()).First(),
            _ => enemies[0]
        };
    }

    void Shoot(GameObject target)
    {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.GetComponent<Projectile>().Initialize(target, CalculateDamage());
    }

    float CalculateDamage()
    {
        bool isCrit = Random.value < CurrentStats().critChance;
        return isCrit
            ? CurrentStats().damage * data.critMultiplier
            : CurrentStats().damage;
    }

    void OnDrawGizmosSelected()
    {
        if (data == null || data.levels.Length == 0) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, CurrentStats().range);
    }
}