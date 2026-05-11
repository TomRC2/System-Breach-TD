using UnityEngine;

public class ComputerPoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null) enemy.ReachComputer();
    }
}