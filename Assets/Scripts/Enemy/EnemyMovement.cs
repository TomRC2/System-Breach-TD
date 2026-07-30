using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 3f; // velocidad base
    private Transform[] waypoints;
    private int currentWaypoint = 0;

    // --- Ralentizacion (efecto de estado) ---
    private float slowMultiplier = 1f;
    private float slowTimer = 0f;

    public int CurrentWaypointIndex() => currentWaypoint;
    public bool IsSlowed() => slowTimer > 0f;
    public float EffectiveSpeed => speed * slowMultiplier;

    public void SetWaypoints(Transform[] points)
    {
        waypoints = points;
    }

    // amount: fraccion de reduccion (0.3 = 30% mas lento). Se queda el slow mas fuerte.
    public void ApplySlow(float amount, float duration)
    {
        float mult = 1f - Mathf.Clamp01(amount);
        if (mult < slowMultiplier) slowMultiplier = mult;
        slowTimer = Mathf.Max(slowTimer, duration);
    }

    public Transform GetCurrentWaypoint()
    {
        if (waypoints == null || currentWaypoint >= waypoints.Length)
            return transform;
        return waypoints[currentWaypoint];
    }

    void Update()
    {
        if (slowTimer > 0f)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0f) slowMultiplier = 1f;
        }

        if (waypoints == null || currentWaypoint >= waypoints.Length) return;

        Transform target = waypoints[currentWaypoint];
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            EffectiveSpeed * Time.deltaTime
        );

        if ((transform.position - target.position).sqrMagnitude < 0.1f * 0.1f)
        {
            currentWaypoint++;
        }

        Vector3 dir = target.position - transform.position;
        if (Mathf.Abs(dir.x) > 0.01f)
        {
            Vector3 scale = transform.localScale;
            scale.x = dir.x < 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }
}
