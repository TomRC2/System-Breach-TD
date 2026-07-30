using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Referencias")]
    public Canvas canvas;
    public Slider slider;

    [Header("Settings")]
    public float hideDelay = 2f;
    public Vector3 offset = new Vector3(0f, 0.8f, 0f);

    private Transform target;
    private Coroutine hideCoroutine;

    void Awake()
    {
        canvas.worldCamera = Camera.main;
        canvas.gameObject.SetActive(false);
    }

    public void Setup(Transform enemyTransform, float maxHP)
    {
        target = enemyTransform;
        slider.maxValue = maxHP;
        slider.value = maxHP;
    }

    public void UpdateHP(float currentHP)
    {
        if (!OptionsManager.IsHealthBarEnabled()) return;

        slider.value = currentHP;
        canvas.gameObject.SetActive(true);

        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    void Update()
    {
        if (target != null)
            transform.position = target.position + offset;
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);
        canvas.gameObject.SetActive(false);
    }
}
