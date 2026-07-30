using UnityEngine;

// Feedback visual de enemigos: parpadeo rojizo al recibir danio
// y tinte azulado mientras estan ralentizados.
// Se aniade por codigo desde EnemyHealth, no hace falta tocar los prefabs.
public class EnemyVisualFX : MonoBehaviour
{
    private static readonly Color flashColor = new Color(1f, 0.35f, 0.35f);
    private static readonly Color slowColor = new Color(0.55f, 0.75f, 1f);

    private SpriteRenderer[] sprites;
    private Color[] originalColors;
    private EnemyMovement movement;
    private float flashTimer;

    void Awake()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
            originalColors[i] = sprites[i].color;
        movement = GetComponent<EnemyMovement>();
    }

    public void Flash(float duration = 0.08f)
    {
        flashTimer = duration;
    }

    void LateUpdate()
    {
        bool flashing = flashTimer > 0f;
        if (flashing) flashTimer -= Time.deltaTime;

        bool slowed = movement != null && movement.IsSlowed();

        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] == null) continue;
            if (flashing) sprites[i].color = originalColors[i] * flashColor;
            else if (slowed) sprites[i].color = originalColors[i] * slowColor;
            else sprites[i].color = originalColors[i];
        }
    }
}
