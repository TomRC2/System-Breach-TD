using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum FocusMode { Closest, Farthest, MostHP, LeastHP, Fastest }

public class TowerController : MonoBehaviour
{
    private TowerData data;
    public FocusMode focusMode = FocusMode.Closest;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public TowerData GetData() => data;

    private float attackCooldown = 0f;

    public void Initialize(TowerData towerData)
    {
        data = towerData;
    }

    void Update()
    {
        if (data == null) return;

        attackCooldown -= Time.deltaTime;

        if (attackCooldown <= 0f)
        {
            GameObject target = GetTarget();
            if (target != null)
            {
                Shoot(target);
                attackCooldown = 1f / data.attackSpeed;
            }
        }
    }

    GameObject GetTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, data.range);
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
            _ => enemies[0]
        };
    }

    void Shoot(GameObject target)
    {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));

        GameObject proj = Instantiate(projectilePrefab,
            firePoint.position, Quaternion.identity);
        proj.GetComponent<Projectile>().Initialize(target, CalculateDamage());
    }

    float CalculateDamage()
    {
        bool isCrit = Random.value < data.critChance;
        return isCrit ? data.damage * data.critMultiplier : data.damage;
    }

    void OnDrawGizmosSelected()
    {
        if (data == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, data.range);
    }
}