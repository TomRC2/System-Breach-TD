using System;
using System.Collections;
using UnityEngine;

public class ComputerHealth : MonoBehaviour
{
    public float maxHP = 1000f;
    public float currentHP;
    public event Action<float, float> OnHPChanged;

    [Header("Game Feel")]
    [Tooltip("Intensidad del temblor de camara al recibir danio")]
    public float shakeIntensity = 0.15f;
    public float shakeDuration = 0.25f;

    private Coroutine shakeRoutine;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        currentHP = Mathf.Max(currentHP - amount, 0f);
        OnHPChanged?.Invoke(currentHP, maxHP); // actualizar UI antes de terminar la partida

        if (Camera.main != null)
        {
            if (shakeRoutine != null) StopCoroutine(shakeRoutine);
            shakeRoutine = StartCoroutine(ShakeCamera(Camera.main.transform));
        }

        if (currentHP <= 0) GameOver();
    }

    IEnumerator ShakeCamera(Transform cam)
    {
        Vector3 basePos = cam.localPosition;
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float falloff = 1f - (elapsed / shakeDuration);
            cam.localPosition = basePos + UnityEngine.Random.insideUnitSphere * shakeIntensity * falloff;
            yield return null;
        }
        cam.localPosition = basePos;
        shakeRoutine = null;
    }

    void GameOver()
    {
        GameManager.Instance.GameOver();
    }
}