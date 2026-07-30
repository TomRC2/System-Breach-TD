using UnityEngine;
using UnityEngine.UI;

public class SpeedToggle : MonoBehaviour
{
    private bool isFast = false;

    [Header("Atajo de teclado")]
    public KeyCode toggleKey = KeyCode.F;

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            Toggle();
    }

    public void Toggle()
    {
        isFast = !isFast;
        Time.timeScale = isFast ? 2f : 1f;
    }
}