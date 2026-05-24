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
    }
}
