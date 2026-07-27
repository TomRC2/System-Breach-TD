using UnityEngine;

public class EnemyMenu: MonoBehaviour
{
    public Transform[] waypoints = new Transform[4];

    public float speed = 5.0f;
    public float rotationSpeed = 8.0f;
    public float reachDistance = 0.2f;

    private int currentIndex = 0;

    void Update()
    {
        if (waypoints.Length == 0 || waypoints[currentIndex] == null) return;

        Transform targetWaypoint = waypoints[currentIndex];

        Vector3 direction = targetWaypoint.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetWaypoint.position) <= reachDistance)
        {
            currentIndex = (currentIndex + 1) % waypoints.Length;
        }
    }
}