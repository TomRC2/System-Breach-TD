using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 3f;
    private Transform[] waypoints;
    private int currentWaypoint = 0;
    public int CurrentWaypointIndex() => currentWaypoint;
    public void SetWaypoints(Transform[] points)
    {
        waypoints = points;
    }

    public Transform GetCurrentWaypoint()
    {
        if (waypoints == null || currentWaypoint >= waypoints.Length)
            return transform;
        return waypoints[currentWaypoint];
    }
    void Update()
    {
        if (waypoints == null || currentWaypoint >= waypoints.Length) return;

        Transform target = waypoints[currentWaypoint];
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
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
