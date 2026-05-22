using UnityEngine;
using UnityEngine.UI;

public class SpeedToggle : MonoBehaviour
{
    private bool isFast = false;

    public void Toggle()
    {
        isFast = !isFast;
        Time.timeScale = isFast ? 2f : 1f;
    }
}