using System.Collections;
using UnityEngine;

// Efecto de escala al colocar ("pop") y al mejorar ("pulso") una torre.
// Se aniade por codigo desde GridCell, no hace falta tocar los prefabs.
public class TowerScaleFX : MonoBehaviour
{
    private Vector3 baseScale;
    private Coroutine current;

    void Awake()
    {
        baseScale = transform.localScale;
    }

    public void PlaySpawn()
    {
        Restart(SpawnRoutine());
    }

    public void PlayUpgrade()
    {
        Restart(PulseRoutine(1.2f, 0.2f));
    }

    void Restart(IEnumerator routine)
    {
        if (current != null) StopCoroutine(current);
        transform.localScale = baseScale;
        current = StartCoroutine(routine);
    }

    // Crece desde 0 con un pequenio rebote (ease-out-back)
    IEnumerator SpawnRoutine()
    {
        float duration = 0.25f;
        float elapsed = 0f;
        transform.localScale = Vector3.zero;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float s = 1.70158f;
            float back = 1f + (s + 1f) * Mathf.Pow(t - 1f, 3f) + s * Mathf.Pow(t - 1f, 2f);
            transform.localScale = baseScale * back;
            yield return null;
        }

        transform.localScale = baseScale;
        current = null;
    }

    // Pulso: sube hasta 'peak' y vuelve
    IEnumerator PulseRoutine(float peak, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float k = 1f + (peak - 1f) * Mathf.Sin(t * Mathf.PI);
            transform.localScale = baseScale * k;
            yield return null;
        }

        transform.localScale = baseScale;
        current = null;
    }
}
